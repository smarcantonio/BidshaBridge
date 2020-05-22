using Newtonsoft.Json;

namespace DeepStackClient
{
    public class IdentifiedScene
    {
        [JsonConstructor]
        public IdentifiedScene(
            [JsonProperty("success")] bool success,
            [JsonProperty("label")] string label,
            [JsonProperty("confidence")] float confidence)
        {
            Success = success;
            Label = label;
            Confidence = confidence;
        }

        public bool Success { get; }
        public string Label { get; }
        public float Confidence { get; }
    }
}
