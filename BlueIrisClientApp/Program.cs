﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading;
using HomeAssistantClient;
using System.Collections.Generic;
using BlueIrisClient;
using System.Diagnostics;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .Build();

            var cancellationToken = CancellationToken.None;

            using var blueIrisClient = await BlueIrisClient.Client.CreateLoggedInAsync(
                new Uri(config["blueIris:uri"]),
                config["blueIris:username"],
                config["blueIris:password"],
                cancellationToken);

            using var homeAssistantClient = await HomeAssistantClient.Client.CreateLoggedInAsync(
                new Uri(config["mqtt:uri"]),
                config["mqtt:username"],
                config["mqtt:password"],
                "homeassistant",
                cancellationToken);

            using var deepStackClient = await DeepStackClient.Client.CreateLoggedInAsync(
                new Uri(config["deepstack:uri"]),
                config["deepstack:apiKey"]);

            var camerasAndGroups = await blueIrisClient.GetCamerasAndGroups();

            Console.WriteLine("Cameras:");
            if (camerasAndGroups.Data != null)
            {
                foreach (var camera in camerasAndGroups.Data.Where(c => !c.IsGroup))
                    Console.WriteLine($"\t{camera.OptionDisplay} ({camera.OptionValue})");
            }

            Console.WriteLine("Groups:");
            if (camerasAndGroups.Data != null)
            {
                foreach (var group in camerasAndGroups.Data.Where(c => c.IsGroup))
                    Console.WriteLine($"\t{group.OptionDisplay} ({group.OptionValue})");
            }

            CameraOrGroup? selectedCamera = null;
            while (true)
            {
                switch (selectedCamera == null ? ConsoleKey.S : Console.ReadKey(true).Key)
                {
                    case ConsoleKey.S:
                        Console.WriteLine("Enter short name of camera");
                        var enteredText = Console.ReadLine();
                        var newCamera = camerasAndGroups.Data.FirstOrDefault(c => !c.IsGroup && string.Equals(c.OptionValue, enteredText, StringComparison.InvariantCultureIgnoreCase));
                        if (!string.IsNullOrWhiteSpace(newCamera?.OptionValue))
                        {
                            selectedCamera = newCamera;
                            Console.WriteLine($"Camera set to '{selectedCamera.OptionValue}'");
                        }
                        else
                        {
                            Console.WriteLine($"Could not find camera called '{enteredText}'");
                            continue;
                        }
                        break;
                    case ConsoleKey.T:
                        Console.WriteLine($"Triggering '{selectedCamera!.OptionValue}");
                        await blueIrisClient.CamSet(selectedCamera!.OptionValue!, trigger: true);
                        break;
                    case ConsoleKey.I:
                        Console.WriteLine($"Capturing image from {selectedCamera!.OptionValue}");
                        var jpegBytes = await blueIrisClient.GetImage(selectedCamera!.OptionValue!);
                        var analysis = await deepStackClient.IdentifyObjects(jpegBytes);
                        Console.WriteLine(analysis);
                        break;
                    case ConsoleKey.M:
                        Console.WriteLine($"Capturing mjpeg image sequence from {selectedCamera!.OptionValue}");
                        await foreach(var imageStream in blueIrisClient.GetImageStream(selectedCamera!.OptionValue, 2))
                        {
                            var sw = Stopwatch.StartNew();
                            var analysis2 = await deepStackClient.IdentifyObjects(imageStream);
                            Console.WriteLine($"Analysed image in {sw.Elapsed}. Predictions:");
                            foreach(var prediction in analysis2.Predictions)
                            {
                                Console.WriteLine($"\t{prediction.Label} ({prediction.Confidence} %)");
                            }
                        }
                        break;
                    case ConsoleKey.P:
                        Console.WriteLine($"Publishing MQTT message to set motion trigger for '{selectedCamera!.OptionValue}'");
                        await homeAssistantClient.SetMqttBinarySensorStateAsync($"{selectedCamera.OptionValue}_motion".ToLowerInvariant(), BinarySensorState.On);
                        break;
                    case ConsoleKey.Q:
                        Console.WriteLine($"Publishing MQTT message to clear motion trigger for '{selectedCamera!.OptionValue}'");
                        await homeAssistantClient.SetMqttBinarySensorStateAsync($"{selectedCamera.OptionValue}_motion".ToLowerInvariant(), BinarySensorState.Off);
                        break;
                    case ConsoleKey.A:
                        Console.WriteLine($"Publishing MQTT message to set attribute for '{selectedCamera!.OptionValue}'");
                        await homeAssistantClient.SetMqttSensorAttributesAsync($"{selectedCamera.OptionValue}_motion".ToLowerInvariant(), new Dictionary<string, object?> { { "humans", 1 }, { "dogs", 2 } });
                        break;
                    case ConsoleKey.V:
                        Console.WriteLine($"Publishing MQTT message to set availability online for '{selectedCamera!.OptionValue}'");
                        await homeAssistantClient.SetMqttSensorAvailabilityAsync($"{selectedCamera.OptionValue}_motion".ToLowerInvariant(), BinarySensorAvailability.Online);
                        break;
                    case ConsoleKey.C:
                        Console.WriteLine($"Publishing MQTT message containing config to auto-discover '{selectedCamera!.OptionValue}'");
                        await homeAssistantClient.ConfigureMqttBinarySensor(
                            $"{selectedCamera!.OptionDisplay ?? selectedCamera!.OptionValue} Motion",
                            $"{selectedCamera!.OptionValue}_motion",
                            selectedCamera!.OptionDisplay ?? selectedCamera!.OptionValue!,
                            selectedCamera!.OptionValue!,
                            DeviceClass.Motion);
                        break;
                    default:
                        break;
                }
                Console.WriteLine("'S' to select camera");
                Console.WriteLine("'T' to trigger");
                Console.WriteLine("'I' to get image");
                Console.WriteLine("'M' to get mjpeg");
                Console.WriteLine("'P' to publish movement:on to MQTT");
                Console.WriteLine("'Q' to publish movement:off to MQTT");
                Console.WriteLine("'A' to publish attributes to MQTT");
                Console.WriteLine("'V' to publish availability:online to MQTT");
                Console.WriteLine("'C' to publish auto-discovery config to MQTT");
            }
        }
    }
}
