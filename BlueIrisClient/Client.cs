using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BlueIrisClient
{

    public class Client : IDisposable
    {
        private readonly HttpClient _jsonClient;
        private readonly HttpClient _httpClient;
        private readonly string _username;
        private readonly string _password;
        private static readonly JsonSerializerSettings _jsonSettings;
        private string? _session;

        static Client()
        {
            _jsonSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new[]
                {
                    new CameraOrGroupJsonConverter()
                }
            };
        }

        private Client(Uri baseUri, string username, string password)
        {
            _username = username;
            _password = password;

            _jsonClient = new HttpClient()
            {
                BaseAddress = baseUri
            };

            _jsonClient.DefaultRequestHeaders.Accept.Clear();
            _jsonClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient = new HttpClient()
            {
                BaseAddress = baseUri
            };

            var basicAuthBytes = Encoding.ASCII.GetBytes($"{username}:{password}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(basicAuthBytes));
        }

        public static async Task<Client> CreateLoggedInAsync(
            Uri baseUri, string username, string password,
            CancellationToken cancellationToken = default)
        {
            var client = new Client(baseUri, username, password);
            try
            {
                var loginResponse = await client.LoginAsync(cancellationToken);
                if (loginResponse.Result == ResponseResult.Success && loginResponse.Session != null)
                {
                    client._session = loginResponse.Session;
                    return client;
                }
                else
                {
                    throw new BlueIrisClientException("Failed to log into Blue Iris");
                }
            }
            catch
            {
                client.Dispose();
                throw;
            }
        }

        private async Task<TResponse> PostJsonAsync<TResponse>(
            object request, CancellationToken cancellationToken = default) where TResponse : Response
        {
            using var content = new StringContent(JsonConvert.SerializeObject(request, _jsonSettings));

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "json")
            {
                Content = content
            };

            using var response = await _jsonClient.SendAsync(httpRequest, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new BlueIrisClientException(responseContent);
            }
            return JsonConvert.DeserializeObject<TResponse>(responseContent, _jsonSettings)!;
        }

        private async Task<Response<LoginResponse>> LoginAsync(CancellationToken cancellationToken = default)
        {
            var loginResponse = await PostJsonAsync<Response<LoginResponse>>(new
            {
                cmd = "login"
            }, cancellationToken);

            if (loginResponse.Result != ResponseResult.Fail)
                throw new BlueIrisClientException($"Unexpected response");

            using var md5 = MD5.Create();

            var authHash = BitConverter.ToString(
               md5.ComputeHash(Encoding.UTF8.GetBytes($"{_username}:{loginResponse.Session}:{_password}")))
               .Replace("-", string.Empty);

            return await PostJsonAsync<Response<LoginResponse>>(new
            {
                cmd = "login",
                session = loginResponse.Session,
                response = authHash
            }, cancellationToken);
        }

        /// <returns>
        /// A list of cameras on the system ordered by group.
        /// Cameras not belonging to any group are shown beneath the "all cameras" group.
        /// Disabled cameras are placed at the end of the list.
        /// </returns>
        public async Task<Response<IEnumerable<CameraOrGroupBase>>> GetCamerasAndGroups(
            bool resetStatCounts = false, bool resetNewAlerts = false,
            CancellationToken cancellationToken = default)
        {
            int reset = (resetStatCounts ? 1 : 0) + (resetNewAlerts ? 2 : 0);
            var response = await PostJsonAsync<Response<IEnumerable<CameraOrGroupBase>>>(new
            {
                cmd = "camlist",
                session = _session,
                reset = reset > 0 ? (int?)reset : default
            }, cancellationToken);
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
            int? move = null,
            CancellationToken cancellationToken = default)
        {
            var zoomArray = zoom.HasValue ? new float[] { zoom.Value.factor, zoom.Value.x1, zoom.Value.y1, zoom.Value.x2, zoom.Value.y2 } : null;

            await PostJsonAsync<Response>(
                new {
                    cmd = "camset",
                    session = _session,
                    camera,
                    click, audio, delete, ptz, trigger, reset, enable, video, zoom = zoomArray, snapPreset, clearPreset, upPreset, downPreset, target, move
                },
                cancellationToken);
        }

        public async Task Trigger(string camera, CancellationToken cancellationToken = default)
        {
            await PostJsonAsync<Response>(
                new
                {
                    cmd = "trigger",
                    session = _session,
                    camera
                },
                cancellationToken);
        }

        public async Task<byte[]> GetImage(string camera, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync($"/image/{camera}", cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new BlueIrisClientException($"Failed to download image for camera '{camera}' from Blue Iris. HTTP status code: {response.StatusCode}");
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async IAsyncEnumerable<Stream> GetImageStream(string camera, decimal framesPerSecond, [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync($"/mjpg/{camera}?fps={framesPerSecond}", HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new BlueIrisClientException($"Failed to download image for camera '{camera}' from Blue Iris. HTTP status code: {response.StatusCode}");
            if (!TryGetMultipartBoundary(response.Content, out var boundary))
                throw new BlueIrisClientException($"Could not extract multi-part data from mjpeg response stream");

            var multipartReader = new MultipartReader(boundary, await response.Content.ReadAsStreamAsync());
            var frame = await multipartReader.ReadNextSectionAsync(cancellationToken);
            while (frame != null)
            {
                yield return frame.Body;
                frame = await multipartReader.ReadNextSectionAsync(cancellationToken);
            }
        }

        private static bool TryGetMultipartBoundary(HttpContent content, [MaybeNullWhen(false)]out string boundary)
        {
            foreach(var header in content.Headers)
            {
                if (!header.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                foreach (var value in header.Value)
                {
                    var parts = value.Split(";");
                    if (parts.Length != 2)
                        continue;
                    if (!parts[0].Equals("multipart/x-mixed-replace"))
                        continue;
                    if (!parts[1].StartsWith("boundary=", StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    boundary = parts[1].Substring("boundary=".Length);
                    return true;
                }
            }
            boundary = default;
            return false;
        }

        public void Dispose()
        {
            _jsonClient.Dispose();
            _httpClient.Dispose();
        }
    }
}
