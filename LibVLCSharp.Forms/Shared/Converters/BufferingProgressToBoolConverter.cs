using System;
using System.Globalization;
using Xamarin.Forms;

namespace LibVLCSharp.Forms.Shared.Converters
{
    /// <summary>
    /// Converts a value not equals to 0 and 1 to true.
    /// </summary>
    internal class BufferingProgressToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>true if value is not equals to 0 or 1, false otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is double d && d != 0 && d != 1;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
