using System.Runtime.InteropServices;
using System;

namespace LibVLCSharp
{
    /// <summary>
    /// Media Player timer point
    /// <br/>
    /// <see cref="Time"/> and <see cref="SystemDate"/> values should not be used directly by the user.
    /// <see cref="MediaPlayer.Interpolate(TimePoint, long, ref long, ref double)" /> will read these values and return an interpolated time.
    /// Also see <see cref="MediaPlayer.WatchTimeOnUpdate"/>
    /// </summary>
    public readonly struct TimePoint
    {
        /// <summary>
        /// Position in the range [0.0f;1.0]
        /// </summary>
        public double Position { get; }

        /// <summary>
        /// Rate of the player
        /// </summary>
        public double Rate { get; }

        /// <summary>
        /// Valid time, in microsecond >= 0 or -1
        /// </summary>
        public long Time { get; }

        /// <summary>
        /// Valid length, in microsecond >= 1 or 0
        /// </summary>
        public long Length { get; }

        /// <summary>
        /// System date, in microsecond, of this record (always valid).
        /// Based on <see cref="LibVLC.Clock"/>. This date can be in the future or in the past.
        /// The special value of INT64_MAX mean that the clock was paused when this point was updated. 
        /// In that case, <see cref="MediaPlayer.Interpolate(TimePoint, long, ref long, ref double)"/> will return the current 
        /// ts/pos of this point (there is nothing to interpolate).
        /// </summary>
        public long SystemDate { get; }
    }
}
