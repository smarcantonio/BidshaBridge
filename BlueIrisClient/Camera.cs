using System;
using System.Drawing;
using Newtonsoft.Json;

namespace BlueIrisClient
{
    public class Camera : CameraOrGroupBase
    {
        [JsonConstructor]
        public Camera(
            [JsonProperty("optionDisplay", Required = Required.Always)] string displayName,
            [JsonProperty("optionValue", Required = Required.Always)] string shortName,
            [JsonProperty("FPS")] decimal? framesPerSecond,
            [JsonProperty("isMotion")] bool? isMotion,
            [JsonProperty("isTriggered")] bool? isTriggered,
            [JsonProperty("audio")] bool? isAudioSupported,
            [JsonProperty("width")] int? width,
            [JsonProperty("height")] int? height,
            [JsonProperty("newalerts")] int? newAlertsCount,
            [JsonProperty("lastalert")] int? lastAlert,
            [JsonProperty("lastalertutc")] long? lastAlertUtc,

            [JsonProperty("color")] int? color,
            [JsonProperty("clipsCreated")] int? clipsCreatedCount,
            [JsonProperty("isAlerting")] bool? isAlerting,
            [JsonProperty("active")] bool? isActive,
            [JsonProperty("type")] CameraType? type,
            [JsonProperty("pause")] int? pause,
            [JsonProperty("isEnabled")] bool? isEnabled,
            [JsonProperty("isOnline")] bool? isOnline,
            [JsonProperty("isNoSignal")] bool? isNoSignal,
            [JsonProperty("isPaused")] bool? isPaused,
            [JsonProperty("isRecording")] bool? isRecording,
            [JsonProperty("isManRec")] bool? isManuallyRecording,
            [JsonProperty("ManRecElapsed")] int? manualRecordingElapsedMs,
            [JsonProperty("ManRecLimit")] int? manualRecordingLimitMs,
            [JsonProperty("isYellow")] bool? isYellow,
            [JsonProperty("profile")] int? profile,
            [JsonProperty("ptz")] bool? isPanTiltZoomSupported,
            [JsonProperty("nTriggers")] int? triggerEventCount,
            [JsonProperty("nNoSignal")] int? noSignalEventCount,
            [JsonProperty("nClips")] int? clipsCount,
            [JsonProperty("error")] string? error)
            : base (
                  displayName: displayName,
                  shortName: shortName,
                  framesPerSecond: framesPerSecond,
                  isMotion: isMotion,
                  isTriggered: isTriggered,
                  isAudioSupported: isAudioSupported,
                  width: width,
                  height: height,
                  newAlertsCount: newAlertsCount,
                  lastAlert: lastAlert,
                  lastAlertUtc: lastAlertUtc)
        {
            Color = color.HasValue
                ? System.Drawing.Color.FromArgb(color.Value & 0xff, (color.Value >> 8) & 0xff, (color.Value >> 16) & 0xff)
                : default;
            ClipsCreatedCount = clipsCreatedCount;
            IsAlerting = isAlerting;
            Type = type;
            Pause = pause;
            IsActive = isActive;
            IsEnabled = isEnabled;
            IsOnline = isOnline;
            IsNoSignal = isNoSignal;
            IsPaused = isPaused;
            IsRecording = isRecording;
            IsManuallyRecording = isManuallyRecording;
            ManualRecordingElapsed = manualRecordingElapsedMs.HasValue
                ? TimeSpan.FromMilliseconds(manualRecordingElapsedMs.Value)
                : default;
            ManualRecordingLimit = manualRecordingLimitMs.HasValue
                ? TimeSpan.FromMilliseconds(manualRecordingLimitMs.Value)
                : default;
            IsYellow = isYellow;
            Profile = profile;
            IsPanTiltZoomSupported = isPanTiltZoomSupported;
            TriggerEventCount = triggerEventCount;
            NoSignalEventCount = noSignalEventCount;
            ClipsCount = clipsCount;
            Error = error;
        }

        /// <summary>
        /// 24-bit RGB value (red least significant) representing the camera's display color
        /// </summary>
        public Color? Color { get; }

        /// <summary>
        /// The number of clips created since the camera stats were last reset
        /// </summary>
        public int? ClipsCreatedCount { get; }

        /// <summary>
        /// Is currently sending an alert
        /// </summary>
        public bool? IsAlerting { get; }

        public bool? IsWebcastEnabled { get; }

        public bool? IsHidden { get; }

        /// <summary>
        /// Is camera temporarily full screen
        /// </summary>
        public bool? IsTempFullscreen { get; }

        /// <summary>
        /// Camera is currently displaying live video
        /// </summary>
        public bool? IsActive { get; }

        public CameraType? Type { get; }

        /// <summary>
        /// 0==not paused, -1==paused indefinitely, else the number of seconds remaining
        /// </summary>
        public int? Pause { get; }

        public bool? IsEnabled { get; }

        public bool? IsOnline { get; }

        public bool? IsPaused { get; }

        public bool? IsNoSignal { get; }

        public bool? IsRecording { get; }

        public bool? IsManuallyRecording { get; }

        /// <summary>
        /// millisecond since manual recording began
        /// </summary>
        public TimeSpan? ManualRecordingElapsed { get; }

        /// <summary>
        /// millisecond limit for a manual recording
        /// </summary>
        public TimeSpan? ManualRecordingLimit { get; }

        public bool? IsYellow { get; }

        /// <summary>
        /// The camera's currently active profile, or as overridden by the global schedule or the UI profile buttons
        /// </summary>
        public int? Profile { get; }

        public bool? IsPanTiltZoomSupported { get; }

        /// <summary>
        /// Number of trigger events since last reset
        /// </summary>
        public int? TriggerEventCount { get; }

        /// <summary>
        /// Number of no signal events since last reset
        /// </summary>
        public int? NoSignalEventCount { get; }

        /// <summary>
        /// Number of no recording events since last reset
        /// </summary>
        public int? ClipsCount { get; }

        /// <summary>
        /// Formatted string with camera error condition
        /// </summary>
        public string? Error { get; }
    }
}
