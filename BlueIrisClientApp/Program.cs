using System;
using BlueIrisClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

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

            while(true)
            {
                Console.WriteLine("Press 'T' to trigger or 'I' to get image");
                switch(Console.ReadKey().Key)
                {
                    case ConsoleKey.T:
                        Console.WriteLine("Triggering Drive");
                        await blueIrisClient.CamSet("Drive", trigger: true);
                        break;
                    case ConsoleKey.I:
                        Console.WriteLine("Capturing image from Drive");
                        await blueIrisClient.GetImage("Drive");
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
