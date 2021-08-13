using System;
using System.Globalization;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared.Converters
{
    /// <summary>
    /// Converts amplification (float) to string.
    /// </summary>
    public class AmplificationToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts an amplification value to string.
        /// Example: -6,23 to 6db.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>the value converted to string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = (int)Math.Round((float)value);
            return result <= 0 ? $"{result}db" : $"+{result}db";
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
