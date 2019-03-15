using System;

namespace LibVLCSharp.Forms
{
    /// <summary>
    /// Extensions methods for <see cref="TimeSpan"/>.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Converts the value of the current <see cref="TimeSpan"/> object to its equivalent short string representation.
        /// </summary>
        /// <param name="span">Time interval.</param>
        /// <returns>The short string representation of the current <see cref="TimeSpan"/> value.</returns>
        public static string ToShortString(this TimeSpan span)
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
