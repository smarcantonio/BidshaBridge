using Newtonsoft.Json;

namespace BlueIrisClient
{
    public class Response
    {
        [JsonProperty("result")]
        public ResponseResult? Result { get; set; }

        [JsonProperty("session")]
        public string? Session { get; set; }
    }

    public class Response<TData> : Response where TData : class
    {
        [JsonProperty("data")]
        public TData? Data { get; set; }
    }
}
