// Imports the namespace for CultureInfo, which provides information about a specific culture.
using System.Globalization;
// Imports the namespace for IValueConverter, a key interface for data binding in WPF.
using System.Windows.Data;

namespace WeatherApp.ViewModel.ValueConverters
{
    /// <summary>
    /// A value converter that transforms a boolean value into a human-readable string indicating if it is raining.
    /// In MVVM, converters are used as a bridge in data binding to display data in a different format
    /// than how it's stored in the ViewModel, without cluttering the ViewModel with presentation-specific logic.
    /// </summary>
    public class BoolToRainConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value from the data source to a string for the UI.
        /// This method is called by the data binding engine when moving data from the ViewModel to the View.
        /// </summary>
        /// <param name="value">The boolean value from the source (e.g., a ViewModel property like 'IsRaining').</param>
        /// <param name="targetType">The type of the binding target property (expected to be string).</param>
        /// <param name="parameter">An optional converter parameter (not used here).</param>
        /// <param name="culture">The culture to use in the converter (not used here).</param>
        /// <returns>A string representation of the weather condition.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Cast the incoming object value to its actual type, a boolean.
            bool isRaining = (bool)value;

            // Check the boolean value and return the appropriate display string.
            if (isRaining)
            {
                // If the source value is 'true', return this string.
                return "Currently raining";
            }
            else
            {
                // If the source value is 'false', return this string.
                return "Currently not raining";
            }
        }

        /// <summary>
        /// Converts a string from the UI back to a boolean value for the data source.
        /// This method is called by the data binding engine in a TwoWay binding scenario when the UI value changes.
        /// Note: This method's implementation logic appears reversed.
        /// </summary>
        /// <param name="value">The string value from the binding target (e.g., from a TextBox).</param>
        /// <param name="targetType">The type to convert to (expected to be boolean).</param>
        /// <param name="parameter">An optional converter parameter (not used here).</param>
        /// <param name="culture">The culture to use in the converter (not used here).</param>
        /// <returns>A boolean value based on the input string.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Cast the incoming object value from the UI to a string.
            string isRaining = (string)value;

            // Check the string value to determine which boolean to return.
            // Note: The logic here returns 'true' when the string is "Currently not raining".
            if (isRaining == "Currently not raining")
            {
                return true;
            }
            else
            {
                // For any other string, including "Currently raining", it returns 'false'.
                return false;
            }
        }
    }
}