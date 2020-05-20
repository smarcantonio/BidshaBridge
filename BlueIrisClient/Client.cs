using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlueIrisClient
{

    public class Client : IDisposable
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerSettings _jsonSettings;
        private string? _session;

        static Client()
        {
            _jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        private Client(Uri baseUri)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = baseUri,
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<Client> CreateLoggedIn(Uri baseUri, string username, string password)
        {
            var client = new Client(baseUri);
            try
            {
                var loginResponse = await client.LoginAsync(username, password);
                if (loginResponse.Result == ResponseResult.Success && loginResponse.Session != null)
                {
                    client._session = loginResponse.Session;
                    return client;
                }
                else
                {
                    throw new Exception("Failed to log into Blue Iris");
                }
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        private async Task<TResponse> PostJsonAsync<TResponse>(object request) where TResponse : Response
        {
            using var content = new StringContent(JsonConvert.SerializeObject(request, _jsonSettings));

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "json")
            {
                Content = content
            };

            using var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(responseContent, _jsonSettings)!;
        }

        private async Task<Response<LoginResponse>> LoginAsync(string username, string password)
        {
            var loginResponse = await PostJsonAsync<Response<LoginResponse>>(new
            {
                cmd = "login"
            });

            if (loginResponse.Result != ResponseResult.Fail)
                throw new Exception($"Unexpected response");

            using var md5 = MD5.Create();

            var authHash = BitConverter.ToString(
               md5.ComputeHash(Encoding.UTF8.GetBytes($"{username}:{loginResponse.Session}:{password}")))
               .Replace("-", string.Empty);

            return await PostJsonAsync<Response<LoginResponse>>(new
            {
                cmd = "login",
                session = loginResponse.Session,
                response = authHash
            });
        }

        /// <returns>
        /// A list of cameras on the system ordered by group.
        /// Cameras not belonging to any group are shown beneath the "all cameras" group.
        /// Disabled cameras are placed at the end of the list.
        /// </returns>
        public async Task<Response<IEnumerable<CameraOrGroup>>> GetCameras()
        {
            var response = await PostJsonAsync<Response<IEnumerable<CameraOrGroup>>>(new
            {
                cmd = "camlist",
                session = _session
            });
            return response;
        }

        /// <summary>
        /// camera window manipulation, added recently for Remote Management
        /// </summary>
        /// <param name="camera">camera short name</param>
        /// <param name="click">perform camera “click” function, which is to select the camera and then reset new alerts for the current user</param>
        /// <param name="audio">true/false, play live audio</param>
        /// <param name="delete">true, delete the camera window</param>
        /// <param name="ptz">a string in the format "id:args". a list of ids is available upon request from support, 2201 - 2240: call preset position 1-40, 2301 - 2340: SET preset position 1-40</param>
        /// <param name="trigger">true, trigger the camera</param>
        /// <param name="reset">true, reset the camera window</param>
        /// <param name="enable">true/false, enable or disable the camera</param>
        /// <param name="video">toggle manual video recording</param>
        /// <param name="zoom">an array of 5 floats defining the zoom factor and X,Y,X2,Y2 zoom view rectangle within the image rectangle</param>
        /// <param name="snapPreset">integer x 1-40, the preset number; capture a preset position image</param>
        /// <param name="clearPreset">integer x 1-40, the preset number; clear a preset position image</param>
        /// <param name="upPreset">integer x 2-40, the preset number; exchange preset values & settings with the one previous (x-1)</param>
        /// <param name="downPreset">preset integer x 1-39, the preset number; exchange preset values & settings with the one following (x+1)</param>
        /// <param name="target">a target camera short name for use by move</param>
        /// <param name="move">0: swap selected camera with target camera window, 1: insert selected camera at target camera window position</param>
        public async Task CamSet(
            string camera,
            bool? click = null,
            bool? audio = null,
            bool? delete = null,
            string? ptz = null,
            bool? trigger = null,
            bool? reset = null,
            bool? enable = null,
            bool? video = null,
            (float factor, float x1, float y1, float x2, float y2)? zoom = null,
            int? snapPreset = null,
            int? clearPreset = null,
            int? upPreset = null,
            int? downPreset = null,
            string? target = null,
            int? move = null)
        {
            var zoomArray = zoom.HasValue ? new float[] { zoom.Value.factor, zoom.Value.x1, zoom.Value.y1, zoom.Value.x2, zoom.Value.y2 } : null;

            await PostJsonAsync<Response>(new {
                cmd = "camset",
                session = _session,
                camera,
                click, audio, delete, ptz, trigger, reset, enable, video, zoom = zoomArray, snapPreset, clearPreset, upPreset, downPreset, target, move
                });
        }

        public async Task Trigger(string camera)
        {
            await PostJsonAsync<Response>(new
            {
                cmd = "trigger",
                session = _session,
                camera
            });
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
