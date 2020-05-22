using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DeepStackClient
{
    public class Client : IDisposable
    {
        private readonly HttpClient _httpClient;

        private Client(Uri uri)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = uri
            };
        }

        // Not really necessary but it follows the pattern of the other two clients
        public static async Task<Client> CreateLoggedInAsync(Uri uri)
        {
            return new Client(uri);
        }

        private async Task<T> IdentifyImage<T>(byte[] jpegImageData, string mode)
        {
            using var request = new MultipartFormDataContent
            {
                { new ByteArrayContent(jpegImageData), "image", "image.jpg" }
            };
            var output = await _httpClient.PostAsync($"v1/vision/{mode}", request);
            var jsonString = await output.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public async Task<IdentifiedScene> IdentifyScene(byte[] jpegImageData)
        {
            return await IdentifyImage<IdentifiedScene>(jpegImageData, "scene");
        }

        public async Task<IdentifiedObjects> IdentifyObjects(byte[] jpegImageData)
        {
            return await IdentifyImage<IdentifiedObjects>(jpegImageData, "detection");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
