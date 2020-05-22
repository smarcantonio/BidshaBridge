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
            Label = label;
            Confidence = confidence;
        }

        public string? Label { get; }
        public float? Confidence { get; }
        public string? Error { get; }
    }
}
