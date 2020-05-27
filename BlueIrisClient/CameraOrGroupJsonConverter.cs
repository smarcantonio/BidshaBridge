using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueIrisClient
{
    public class CameraOrGroupJsonConverter : JsonConverter
    {
        public sealed override object? ReadJson(
            JsonReader reader,
            Type objectType,
            object? existingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var groupToken = jObject.SelectToken("group");
            var deserialized = groupToken == null
                ? (CameraOrGroupBase?)jObject.ToObject<Camera>(serializer)
                : jObject.ToObject<Group>(serializer);
            if (deserialized == null)
                throw new BlueIrisClientException("Failed to deserialize camera or group");
            return deserialized;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(CameraOrGroupBase));
        }

        public override bool CanWrite => false;
    }
}
