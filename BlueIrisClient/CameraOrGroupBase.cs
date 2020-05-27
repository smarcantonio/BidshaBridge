using System.Drawing;

namespace BlueIrisClient
{
    public abstract class CameraOrGroupBase
    {
        public CameraOrGroupBase(
            string displayName,
            string shortName,
            decimal? framesPerSecond,
            bool? isMotion,
            bool? isTriggered,
            bool? isAudioSupported,
            int? width,
            int? height,
            int? newAlertsCount,
            int? lastAlert,
            long? lastAlertUtc)
        {
            DisplayName = displayName;
            ShortName = shortName;
            FramesPerSecond = framesPerSecond;
            IsMotion = isMotion;
            IsTriggered = isTriggered;
            IsAudioSupported = isAudioSupported;
            FrameSizePixels = (width.HasValue || height.HasValue) ? new Size(width ?? 0, height ?? 0) : default;
            NewAlertsCount = newAlertsCount;
            LastAlert = lastAlert;
            LastAlertUtc = lastAlertUtc;
        }

        /// <summary>
        /// The camera or group name
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The camera or group short name, used for other requests and commands requiring a camera short name
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// The current number of frames/second delivered from the camera
        /// </summary>
        public decimal? FramesPerSecond { get; }

        /// <summary>
        /// Current sensing motion
        /// </summary>
        public bool? IsMotion { get; }

        public bool? IsTriggered { get; }

        public bool? IsAudioSupported { get; }

        /// <summary>
        /// Frame pixel size
        /// </summary>
        public Size? FrameSizePixels { get; }

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
    }
}
