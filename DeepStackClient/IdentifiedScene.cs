using Newtonsoft.Json;

namespace DeepStackClient
{
    public class IdentifiedScene
    {
        [JsonConstructor]
        public IdentifiedScene(
            [JsonProperty("label")] string label,
            [JsonProperty("confidence")] float confidence)
        {
            Label = label ?? string.Empty;
            Confidence = confidence;
        }

        public string Label { get; }
        public float Confidence { get; }
    }
}
