using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeepStackClient
{
    public class IdentifiedObjects
    {
        [JsonConstructor]
        public IdentifiedObjects(
            [JsonProperty("predictions")] IReadOnlyList<IdentifiedObject> predictions)
        {
            Predictions = predictions ?? Array.Empty<IdentifiedObject>();
        }

        public IReadOnlyList<IdentifiedObject> Predictions { get; }
    }
}
