using System;

namespace LibVLCSharp.Shared.MediaPlayerElement
{
    /// <summary>
    /// Extensions methods for <see cref="TimeSpan"/>
    /// </summary>
    internal static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts the value of the current <see cref="TimeSpan"/> object to its equivalent short string representation
        /// </summary>
        /// <param name="span">time interval</param>
        /// <returns>the short string representation of the current <see cref="TimeSpan"/> value</returns>
        internal static string ToShortString(this TimeSpan span)
        {
            if (span.Days != 0)
            {
                return span.ToString(@"d\.hh\:mm\:ss");
            }
            if (span.Hours != 0)
            {
                return span.ToString(@"hh\:mm\:ss");
            }
            return span.ToString(@"mm\:ss");
        }
    }
}
