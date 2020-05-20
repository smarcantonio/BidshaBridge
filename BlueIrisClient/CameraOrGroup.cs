using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlueIrisClient
{
    public class CameraOrGroup
    {
        [JsonConstructor]
        public CameraOrGroup(
            [JsonProperty("optionDisplay")] string? optionDisplay,
            [JsonProperty("optionValue")] string? optionValue,
            [JsonProperty("FPS")] decimal? framesPerSecond,
            [JsonProperty("color")] int? color,
            [JsonProperty("clipsCreated")] int? clipsCreatedCount,
            [JsonProperty("isAlerting")] bool? isAlerting,
            [JsonProperty("active")] bool? isActive,
            [JsonProperty("type")] CameraType? type,
            [JsonProperty("pause")] int? pause,
            [JsonProperty("isEnabled")] bool? isEnabled,
            [JsonProperty("isOnline")] bool? isOnline,
            [JsonProperty("isMotion")] bool? isMotion,
            [JsonProperty("isNoSignal")] bool? isNoSignal,
            [JsonProperty("isPaused")] bool? isPaused,
            [JsonProperty("isTriggered")] bool? isTriggered,
            [JsonProperty("isRecording")] bool? isRecording,
            [JsonProperty("isManRec")] bool? isManuallyRecording,
            [JsonProperty("ManRecElapsed")] int? manualRecordingElapsedMs,
            [JsonProperty("ManRecLimit")] int? manualRecordingLimitMs,
            [JsonProperty("isYellow")] bool? isYellow,
            [JsonProperty("profile")] int? profile,
            [JsonProperty("ptz")] bool? isPanTiltZoomSupported,
            [JsonProperty("audio")] bool? isAudioSupported,
            [JsonProperty("width")] int? width,
            [JsonProperty("height")] int? height,
            [JsonProperty("nTriggers")] int? triggerEventCount,
            [JsonProperty("nNoSignal")] int? noSignalEventCount,
            [JsonProperty("nClips")] int? clipsCount,
            [JsonProperty("xsize")] int? xSize,
            [JsonProperty("ysize")] int? ySize,
            [JsonProperty("group")] IReadOnlyList<string>? group,
            [JsonProperty("rects")] IReadOnlyList<int[]>? rects,
            [JsonProperty("newalerts")] int? newAlertsCount,
            [JsonProperty("lastalert")] int? lastAlert,
            [JsonProperty("lastalertutc")] long? lastAlertUtc,
            [JsonProperty("error")] string? error)
        {
            OptionDisplay = optionDisplay;
            OptionValue = optionValue;
            FramesPerSecond = framesPerSecond;
            Color = color;
            ClipsCreatedCount = clipsCreatedCount;
            IsAlerting = isAlerting;
            IsActive = isActive;
            Type = type;
            Pause = pause;
            IsEnabled = isEnabled;
            IsOnline = isOnline;
            IsMotion = isMotion;
            IsNoSignal = isNoSignal;
            IsPaused = isPaused;
            IsTriggered = isTriggered;
            IsRecording = isRecording;
            IsManuallyRecording = isManuallyRecording;
            ManualRecordingElapsedMs = manualRecordingElapsedMs;
            ManualRecordingLimitMs = manualRecordingLimitMs;
            IsYellow = isYellow;
            Profile = profile;
            IsPanTiltZoomSupported = isPanTiltZoomSupported;
            IsAudioSupported = isAudioSupported;
            Width = width;
            Height = height;
            TriggerEventCount = triggerEventCount;
            NoSignalEventCount = noSignalEventCount;
            ClipsCount = clipsCount;
            XSize = xSize;
            YSize = ySize;
            Group = group;
            Rects = rects;
            NewAlertsCount = newAlertsCount;
            LastAlert = lastAlert;
            LastAlertUtc = lastAlertUtc;
            Error = error;
        }

        /// <summary>
        /// The camera or group name
        /// </summary>
        public string? OptionDisplay { get; }

        /// <summary>
        /// The camera or group short name, used for other requests and commands requiring a camera short name
        /// </summary>
        public string? OptionValue { get; }

        /// <summary>
        /// The current number of frames/second delivered from the camera
        /// </summary>
        public decimal? FramesPerSecond { get; }

        /// <summary>
        /// 24-bit RGB value (red least significant) representing the camera's display color
        /// </summary>
        public int? Color { get; }

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

        /// <summary>
        /// Current sensing motion
        /// </summary>
        public bool? IsMotion { get; }

        public bool? IsNoSignal { get; }

        public bool? IsPaused { get; }

        public bool? IsTriggered { get; }

        public bool? IsRecording { get; }

        public bool? IsManuallyRecording { get; }

        /// <summary>
        /// millisecond since manual recording began
        /// </summary>
        public int? ManualRecordingElapsedMs { get; }

        /// <summary>
        /// millisecond limit for a manual recording
        /// </summary>
        public int? ManualRecordingLimitMs { get; }

        public bool? IsYellow { get; }

        /// <summary>
        /// The camera's currently active profile, or as overridden by the global schedule or the UI profile buttons
        /// </summary>
        public int? Profile { get; }

        public bool? IsPanTiltZoomSupported { get; }

        public bool? IsAudioSupported { get; }

        /// <summary>
        /// Frame pixel width
        /// </summary>
        public int? Width { get; }

        /// <summary>
        /// Frame pixel height
        /// </summary>
        public int? Height { get; }

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
        /// For a group, the number of cameras across
        /// </summary>
        public int? XSize { get; }

        /// <summary>
        /// For a group, the number of cameras tall
        /// </summary>
        public int? YSize { get; }

        /// <summary>
        /// For a group, an array of the camera short names in the group
        /// </summary>
        public IReadOnlyList<string>? Group { get; }

        /// <summary>
        /// For a group, an array of the camera rectangles within the group image
        /// </summary>
        public IReadOnlyList<int[]>? Rects { get; }

        /// <summary>
        /// Per camera, per user number of new alerts
        /// </summary>
        public int? NewAlertsCount { get; }

        /// <summary>
        /// Database record locator for most recent alert image
        /// </summary>
        public int? LastAlert { get; }

        /// <summary>
        /// UTC timecode (msec precision) for most recent alert image
        /// </summary>
        public long? LastAlertUtc { get; }

        /// <summary>
        /// Formatted string with camera error condition
        /// </summary>
        public string? Error { get; }
    }
}
