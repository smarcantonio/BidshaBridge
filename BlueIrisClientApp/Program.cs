using System;
using BlueIrisClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BlueIrisClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .Build();

            using var blueIrisClient = await Client.CreateLoggedIn(
                new Uri(config["blueIris:uri"]),
                config["blueIris:username"],
                config["blueIris:password"]);

            var cameras = await blueIrisClient.GetCameras();

            Console.WriteLine("Cameras:");
            if (cameras.Data != null)
            {
                foreach (var camera in cameras.Data)
                {
                    Console.WriteLine($"{camera.OptionDisplay} ({camera.OptionValue})");
                }
            }

            while (true)
            {
                Console.WriteLine("Press enter to trigger Drive");
                Console.ReadLine();
//                await blueIrisClient.Trigger("Drive");
                await blueIrisClient.CamSet("Drive", trigger: true);
            }

        }
    }
}
