using System;
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

            var cameras = (await blueIrisClient.GetCamerasAndGroups()).Data.OfType<Camera>().ToList();

            if (cameras == null || cameras.Count == 0)
            {
                Console.WriteLine("No cameras found");
                return;
            }


            CameraOrGroupBase selectedCamera = cameras.First();
            while (true)
            {
                Console.WriteLine($"Selected camera: {selectedCamera.DisplayName} ({selectedCamera.ShortName})");
                Console.WriteLine("'S' to select camera");
                Console.WriteLine("'T' to trigger");
                Console.WriteLine("'I' to get image");
                Console.WriteLine("'M' to get mjpeg");
                Console.WriteLine("'P' to publish movement:on to MQTT");
                Console.WriteLine("'Q' to publish movement:off to MQTT");
                Console.WriteLine("'A' to publish attributes to MQTT");
                Console.WriteLine("'V' to publish availability:online to MQTT");
                Console.WriteLine("'C' to publish auto-discovery config to MQTT");

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.S:
                        {
                            Console.WriteLine("Cameras:");
                            foreach (var camera in cameras)
                                Console.WriteLine($"\t{camera.DisplayName} ({camera.ShortName})");
                            Console.WriteLine("Enter short name of camera:");
                            var enteredText = Console.ReadLine();
                            var newCamera = cameras
                                .FirstOrDefault(c => string.Equals(c.ShortName, enteredText, StringComparison.InvariantCultureIgnoreCase));
                            if (!string.IsNullOrWhiteSpace(newCamera?.ShortName))
                            {
                                selectedCamera = newCamera;
                                Console.WriteLine($"Camera set to '{selectedCamera.ShortName}'");
                            }
                            else
                            {
                                Console.WriteLine($"Could not find camera called '{enteredText}'");
                                continue;
                            }
                        }
                        break;

                    case ConsoleKey.T:
                        {
                            Console.WriteLine($"Triggering '{selectedCamera.ShortName}");
                            await blueIrisClient.CamSet(selectedCamera.ShortName!, trigger: true);
                        }
                        break;

                    case ConsoleKey.I:
                        {
                            Console.WriteLine($"Capturing image from {selectedCamera!.ShortName}");
                            var jpegBytes = await blueIrisClient.GetImage(selectedCamera!.ShortName!);
                            var sw = Stopwatch.StartNew();
                            var analysis = await deepStackClient.IdentifyObjects(jpegBytes);
                            Console.WriteLine($"Analysed image in {sw.Elapsed}. Predictions:");
                            foreach (var prediction in analysis.Predictions)
                            {
                                Console.WriteLine($"\t{prediction.Label} ({prediction.Confidence} %)");
                            }
                        }
                        break;

                    case ConsoleKey.M:
                        {
                            Console.WriteLine($"Capturing mjpeg image sequence from {selectedCamera.ShortName}");
                            await foreach (var imageStream in blueIrisClient.GetImageStream(selectedCamera.ShortName!, 2))
                            {
                                var sw = Stopwatch.StartNew();
                                var analysis = await deepStackClient.IdentifyObjects(imageStream);
                                Console.WriteLine($"Analysed image in {sw.Elapsed}. Predictions:");
                                foreach (var prediction in analysis.Predictions)
                                {
                                    Console.WriteLine($"\t{prediction.Label} ({prediction.Confidence} %)");
                                }
                            }
                        }
                        break;

                    case ConsoleKey.P:
                        {
                            Console.WriteLine($"Publishing MQTT message to set motion trigger for '{selectedCamera!.ShortName}'");
                            await homeAssistantClient.SetMqttBinarySensorStateAsync($"{selectedCamera.ShortName}_motion".ToLowerInvariant(), BinarySensorState.On);
                        }
                        break;

                    case ConsoleKey.Q:
                        {
                            Console.WriteLine($"Publishing MQTT message to clear motion trigger for '{selectedCamera!.ShortName}'");
                            await homeAssistantClient.SetMqttBinarySensorStateAsync($"{selectedCamera.ShortName}_motion".ToLowerInvariant(), BinarySensorState.Off);
                        }
                        break;

                    case ConsoleKey.A:
                        {
                            Console.WriteLine($"Publishing MQTT message to set attribute for '{selectedCamera!.ShortName}'");
                            await homeAssistantClient.SetMqttSensorAttributesAsync($"{selectedCamera.ShortName}_motion".ToLowerInvariant(), new Dictionary<string, object?> { { "humans", 1 }, { "dogs", 2 } });
                        }
                        break;

                    case ConsoleKey.V:
                        {
                            Console.WriteLine($"Publishing MQTT message to set availability online for '{selectedCamera!.ShortName}'");
                            await homeAssistantClient.SetMqttSensorAvailabilityAsync($"{selectedCamera.ShortName}_motion".ToLowerInvariant(), BinarySensorAvailability.Online);
                        }
                        break;

                    case ConsoleKey.C:
                        {
                            Console.WriteLine($"Publishing MQTT message containing config to auto-discover '{selectedCamera!.ShortName}'");
                            await homeAssistantClient.ConfigureMqttBinarySensor(
                                $"{selectedCamera!.DisplayName ?? selectedCamera!.ShortName} Motion",
                                $"{selectedCamera!.ShortName}_motion",
                                selectedCamera!.DisplayName ?? selectedCamera!.ShortName!,
                                selectedCamera!.ShortName!,
                                DeviceClass.Motion);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
