using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;

namespace HomeAssistantClient
{
    public enum DeviceType
    {
        Sensor,
        BinarySensor,
        Switch
    }

    public enum DeviceClass
    {
        Motion
    }

    public enum BinarySensorState
    {
        On,
        Off
    }

    public enum BinarySensorAvailability
    {
        Online,
        Offline
    }

    public class HomeAssistantException : Exception
    {
        public HomeAssistantException(string message) : base(message)
        {

        }
    }

    public class Client : IDisposable
    {
        private readonly IMqttClient _mqttClient;
        private readonly string _mqttDiscoveryPrefix;

        public static async Task<Client> CreateLoggedInAsync(
            Uri mqttUri, string mqttUsername, string mqttPassword,
            string mqttDiscoveryPrefix,
            CancellationToken cancellationToken = default)
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(mqttUri.Host, mqttUri.Scheme switch {"mqtt" => 1883, "mqtts" => 8883 })
                .WithTls(options =>
                {
                    options.UseTls = mqttUri.Scheme switch { "mqtt" => false, "mqtts" => true };
                })
                .WithCredentials(mqttUsername, mqttPassword)
                .Build();

            var connectResult = await mqttClient.ConnectAsync(options, cancellationToken);

            Console.WriteLine(connectResult.ResultCode);

            return new Client(mqttClient, mqttDiscoveryPrefix);
        }

        private Client(IMqttClient mqttClient, string mqttDiscoveryPrefix)
        {
            _mqttClient = mqttClient;
            _mqttDiscoveryPrefix = mqttDiscoveryPrefix;
        }

        private static string BuildMqttTopic(string discoveryPrefix, DeviceType deviceType, string objectId, string property)
        {
            var deviceTypeString = deviceType switch
            {
                DeviceType.Sensor => "sensor",
                DeviceType.BinarySensor => "binary_sensor",
                DeviceType.Switch => "switch"
            };

            return $"{discoveryPrefix}/{deviceTypeString}/{objectId}/{property}".ToLowerInvariant();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="objectId"></param>
        /// <param name="deviceClass"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ConfigureMqttBinarySensor(
            string sensorName,
            string sensorId,
            string deviceName,
            string deviceId,
            DeviceClass deviceClass,
            CancellationToken cancellationToken = default)
        {
            var deviceClassString = deviceClass switch
            {
                DeviceClass.Motion => "motion"
            };

            var publishResult = await _mqttClient.PublishAsync(new MqttApplicationMessage
            {
                Topic = BuildMqttTopic(_mqttDiscoveryPrefix, DeviceType.BinarySensor, sensorId, "config"),
                Payload = Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new
                    {
                        name = sensorName,
                        unique_id = $"binary_sensor.{sensorId.ToLowerInvariant()}",
                        device_class = deviceClassString,
                        state_topic = BuildMqttTopic(_mqttDiscoveryPrefix, DeviceType.BinarySensor, sensorId, "state"),
                        availability_topic = BuildMqttTopic(_mqttDiscoveryPrefix, DeviceType.BinarySensor, sensorId, "availability"),
                        json_attributes_topic = BuildMqttTopic(_mqttDiscoveryPrefix, DeviceType.BinarySensor, sensorId, "attributes"),
                        device = new {
                            name = deviceName,
                            identifiers = deviceId
                        }
                    })),
//                Retain = true,
                }, cancellationToken);

            if (publishResult.ReasonCode != MQTTnet.Client.Publishing.MqttClientPublishReasonCode.Success)
                throw new HomeAssistantException($"Failed to publish MQTT message: {publishResult.ReasonCode} {publishResult.ReasonString}");
        }

        public async Task SetMqttBinarySensorStateAsync(string objectId, BinarySensorState state)
        {
            var topic = BuildMqttTopic(_mqttDiscoveryPrefix, DeviceType.BinarySensor, objectId, "state");
            var publishResult = await _mqttClient.PublishAsync(topic, state.ToString().ToUpperInvariant());

            if (publishResult.ReasonCode != MQTTnet.Client.Publishing.MqttClientPublishReasonCode.Success)
                throw new HomeAssistantException($"Failed to publish MQTT message: {publishResult.ReasonCode} {publishResult.ReasonString}");
        }

        public async Task SetMqttSensorAvailabilityAsync(string objectId, BinarySensorAvailability availability)
        {
            var topic = BuildMqttTopic(_mqttDiscoveryPrefix, DeviceType.BinarySensor, objectId, "availability");
            var publishResult = await _mqttClient.PublishAsync(topic, availability.ToString().ToLowerInvariant());

            if (publishResult.ReasonCode != MQTTnet.Client.Publishing.MqttClientPublishReasonCode.Success)
                throw new HomeAssistantException($"Failed to publish MQTT message: {publishResult.ReasonCode} {publishResult.ReasonString}");
        }

        public async Task SetMqttSensorAttributesAsync(string objectId, IReadOnlyDictionary<string, object?> attributes)
        {
            var topic = BuildMqttTopic(_mqttDiscoveryPrefix, DeviceType.BinarySensor, objectId, "attributes");
            var jsonPayload = JsonConvert.SerializeObject(attributes);
            var publishResult = await _mqttClient.PublishAsync(topic, jsonPayload);

            if (publishResult.ReasonCode != MQTTnet.Client.Publishing.MqttClientPublishReasonCode.Success)
                throw new HomeAssistantException($"Failed to publish MQTT message: {publishResult.ReasonCode} {publishResult.ReasonString}");
        }

        public async Task SetMqttNumericSensorAsync(string objectId, decimal value)
        {
            var topic = BuildMqttTopic(_mqttDiscoveryPrefix, DeviceType.Sensor, objectId, "state");
            var publishResult = await _mqttClient.PublishAsync(topic, value.ToString());

            if (publishResult.ReasonCode != MQTTnet.Client.Publishing.MqttClientPublishReasonCode.Success)
                throw new HomeAssistantException($"Failed to publish MQTT message: {publishResult.ReasonCode} {publishResult.ReasonString}");
        }

        public async Task SetHttpBinarySensorAsync(string objectId, BinarySensorState state)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _mqttClient.Dispose();
        }
    }
}
