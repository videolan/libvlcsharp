using System;
using System.Globalization;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared.Converters
{
    /// <summary>
    /// Convert float value to string.
    /// </summary>
    public class FrequencyToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a frequency value to a string.
        /// Example: 3000 to 3kHz or 20 to 20Hz.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>the value converted to string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var frequency = (float)value;
            return frequency < 1000 ? $"{Math.Round(frequency)}Hz" : $"{Math.Round(frequency / 1000)}Hz";
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
