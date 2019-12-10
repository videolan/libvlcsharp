using System;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Media position
    /// </summary>
    public class MediaPosition
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MediaPosition"/> class
        /// </summary>
        /// <param name="position">position as percentage, between 0.0 and 1.0</param>
        /// <param name="seekBarPosition">seek bar position</param>
        /// <param name="length">media length in milliseconds</param>
        public MediaPosition(float position, double seekBarPosition, long length)
        {
            Position = position;
            SeekBarPosition = seekBarPosition;
            var elapsedTime = position * length;
            ElapsedTime = TimeSpan.FromMilliseconds(elapsedTime);
            RemainingTime = TimeSpan.FromMilliseconds(length - elapsedTime);
        }

        /// <summary>
        /// Gets the media position
        /// </summary>
        public float Position { get; }

        /// <summary>
        /// Gets the seek bar position
        /// </summary>
        public double SeekBarPosition { get; }

        /// <summary>
        /// Gets the elapsed time
        /// </summary>
        public TimeSpan ElapsedTime { get; }

        /// <summary>
        /// Gets the remaining time
        /// </summary>
        public TimeSpan RemainingTime { get; }

        /// <summary>
        /// Gets the elapsed time text
        /// </summary>
        public string ElapsedTimeText => ElapsedTime.ToShortString();

        /// <summary>
        /// Gets the remaining time text
        /// </summary>
        public string RemainingTimeText => RemainingTime.ToShortString();
    }
}
