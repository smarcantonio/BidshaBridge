using System;
using Newtonsoft.Json;

namespace DeepStackClient
{
    public class DeepStackException : Exception
    {
        [JsonConstructor]
        public DeepStackException(
            [JsonProperty("error")] string message) : base(message)
        {
        }
    }
}
