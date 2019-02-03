using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Vs.AddMultipleProjectsToSolution.Gui.Utilities
{
    public class BooleanToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(FontWeight)) throw new NotSupportedException();

            switch (value)
            {
                case bool bValue when bValue:
                    return FontWeights.Bold;
                default:
                    return FontWeights.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is FontWeight))
                throw new NotSupportedException($"Converting from type {value?.GetType()} is not supported.");
            if (targetType != typeof(bool))
                throw new NotSupportedException($"Converting to type {targetType} is not supported.");

            switch (value)
            {
                case FontWeight weight when weight == FontWeights.Bold:
                    return true;
                default:
                    return false;
            }
        }
    }
}