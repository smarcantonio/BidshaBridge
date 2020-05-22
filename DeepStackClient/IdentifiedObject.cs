using System.Drawing;
using Newtonsoft.Json;

namespace DeepStackClient
{
    public class IdentifiedObject
    {
        [JsonConstructor]
        public IdentifiedObject(
            [JsonProperty("label")] string label,
            [JsonProperty("confidence")] float confidence,
            [JsonProperty("y_min")] int yMin,
            [JsonProperty("x_min")] int xMin,
            [JsonProperty("y_max")] int yMax,
            [JsonProperty("x_max")] int xMax)
        {
            Label = label;
            Confidence = confidence;
            Location = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        public string Label { get; }
        public float Confidence { get; }
        public Rectangle Location { get; }
    }
}
