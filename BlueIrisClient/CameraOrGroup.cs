using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
            Color = color.HasValue ? System.Drawing.Color.FromArgb(color.Value & 0xff, (color.Value >> 8) & 0xff, (color.Value >> 16) & 0xff) : default;
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
            ManualRecordingElapsed = manualRecordingElapsedMs.HasValue ? TimeSpan.FromMilliseconds(manualRecordingElapsedMs.Value) : default;
            ManualRecordingLimit = manualRecordingLimitMs.HasValue ? TimeSpan.FromMilliseconds(manualRecordingLimitMs.Value) : default;
            IsYellow = isYellow;
            Profile = profile;
            IsPanTiltZoomSupported = isPanTiltZoomSupported;
            IsAudioSupported = isAudioSupported;
            PixelSize = (width.HasValue || height.HasValue) ? new Size(width ?? 0, height ?? 0) : default;
            TriggerEventCount = triggerEventCount;
            NoSignalEventCount = noSignalEventCount;
            ClipsCount = clipsCount;
            GroupDimensions = (xSize.HasValue || ySize.HasValue) ? new Size(xSize ?? 0, ySize ?? 0) : default;
            Group = group;
            Rects = rects?.Select(r => new Rectangle(r[0], r[1], r[2]-r[0], r[3]-r[1])).ToList();
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

        public bool? IsAudioSupported { get; }

        /// <summary>
        /// Frame pixel size
        /// </summary>
        public Size? PixelSize { get; }

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
        /// For a group, the number of cameras across and tall
        /// </summary>
        public Size? GroupDimensions { get; }

        /// <summary>
        /// For a group, an array of the camera short names in the group
        /// </summary>
        public IReadOnlyList<string>? Group { get; }

        /// <summary>
        /// For a group, an array of the camera rectangles within the group image
        /// </summary>
        public IReadOnlyList<Rectangle>? Rects { get; }

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

        public bool IsGroup => Group != null;
    }
}
