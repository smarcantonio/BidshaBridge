using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeepStackClient
{
    public class Client : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        private Client(Uri uri, string apiKey)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = uri
            };
            _apiKey = apiKey;
        }

        // Not really necessary but it follows the pattern of the other two clients
        public static async Task<Client> CreateLoggedInAsync(Uri uri, string apiKey)
        {
            return new Client(uri, apiKey);
        }

        private async Task<T> IdentifyImage<T>(byte[] jpegImageData, string mode)
        {
            using var request = new MultipartFormDataContent
            {
                { new ByteArrayContent(jpegImageData), "image", "image.jpg" },
                { new StringContent(_apiKey), "api_key" }
            };
            var output = await _httpClient.PostAsync($"v1/vision/{mode}", request);
            var jsonString = await output.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject(jsonString);
            var jObject = JObject.Parse(jsonString);
            if (jObject == null)
                throw new DeepStackException("Failed to parse response");
            if (!jObject.SelectToken("success").Value<bool>())
                throw jObject.ToObject<DeepStackException>();
            
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
