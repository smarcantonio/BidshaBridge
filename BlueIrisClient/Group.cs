using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace BlueIrisClient
{
    public class Group : CameraOrGroupBase
    {
        [JsonConstructor]
        public Group(
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

            [JsonProperty("group")] IReadOnlyList<string>? groupMembers,
            [JsonProperty("xsize")] int? xSize,
            [JsonProperty("ysize")] int? ySize,
            [JsonProperty("rects")] IReadOnlyList<int[]>? rects)
            : base(
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
            GroupMembers = groupMembers ?? Array.Empty<string>();
            GroupDimensions = (xSize.HasValue || ySize.HasValue)
                ? new Size(xSize ?? 0, ySize ?? 0)
                : default;
            Rects = rects?.Select(r => new Rectangle(r[0], r[1], r[2] - r[0], r[3] - r[1])).ToList()
                ?? (IReadOnlyList<Rectangle>)Array.Empty<Rectangle>();
        }

        /// <summary>
        /// For a group, the number of cameras across and tall
        /// </summary>
        public Size? GroupDimensions { get; }

        /// <summary>
        /// For a group, an array of the camera short names in the group
        /// </summary>
        public IReadOnlyList<string> GroupMembers { get; }

        /// <summary>
        /// For a group, an array of the camera rectangles within the group image
        /// </summary>
        public IReadOnlyList<Rectangle> Rects { get; }
    }
}
