using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueIrisClient
{
    public class LoginResponse
    {
        [JsonConstructor]
        public LoginResponse(
            [JsonProperty("system name")] string? systemName,
            [JsonProperty("admin")] bool? admin,
            [JsonProperty("ptz")] bool? ptz,
            [JsonProperty("audio")] bool? audio,
            [JsonProperty("clips")] bool? clips,
            [JsonProperty("streamtimelimit")] bool? streamTimeLimit,
            [JsonProperty("dio")] bool? dio,
            [JsonProperty("version")] string? version,
            [JsonProperty("license")] string? license,
            [JsonProperty("support")] string? support,
            [JsonProperty("user")] string? user,
            [JsonProperty("latitude")] decimal? latitude,
            [JsonProperty("longitude")] decimal? longitude,
            [JsonProperty("tzone")] int? timeZoneUtcOffsetMinutes,
            [JsonProperty("streams")] IReadOnlyList<string>? streams,
            [JsonProperty("sounds")] IReadOnlyList<string>? sounds,
            [JsonProperty("profiles")] IReadOnlyList<string>? profiles,
            [JsonProperty("schedules")] IReadOnlyList<string>? schedules)
        {
            SystemName = systemName;
            Admin = admin;
            Ptz = ptz;
            Audio = audio;
            Clips = clips;
            StreamTimeLimit = streamTimeLimit;
            Dio = dio;
            Version = version;
            License = license;
            Support = support;
            User = user;
            Latitude = latitude;
            Longitude = longitude;
            TimeZoneUtcOffsetMinutes = timeZoneUtcOffsetMinutes;
            Streams = streams;
            Sounds = sounds;
            Profiles = profiles;
            Schedules = schedules;
        }

        public string? SystemName { get; }

        public bool? Admin { get; }

        public bool? Ptz { get; }

        public bool? Audio { get; }

        public bool? Clips { get; }

        public bool? StreamTimeLimit { get; }

        public bool? Dio { get; }

        public string? Version { get; }

        public string? License { get; }

        // "8/3/2020 4:06:24 PM"
        public string? Support { get; }

        public string? User { get; }

        public decimal? Latitude { get; }

        public decimal? Longitude { get; }

        public int? TimeZoneUtcOffsetMinutes { get; }

        public IReadOnlyList<string>? Streams { get; }

        public IReadOnlyList<string>? Sounds { get; }

        public IReadOnlyList<string>? Profiles { get; }

        public IReadOnlyList<string>? Schedules { get; }
    }
}
