using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeepStackClient
{
    public class IdentifiedObjects
    {
        [JsonConstructor]
        public IdentifiedObjects(
            [JsonProperty("success")] bool success,
            [JsonProperty("predictions")] IReadOnlyList<IdentifiedObject> predictions)
        {
            Success = success;
            Predictions = predictions ?? Array.Empty<IdentifiedObject>();
        }

        public bool Success { get; }
        public IReadOnlyList<IdentifiedObject> Predictions { get; }
    }
}
