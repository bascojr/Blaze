using System.Globalization;
using System.Windows.Data;

namespace Blaze.App.Converters;

/// <summary>
/// Converts a boolean to "Free" or the price text
/// </summary>
public class BoolToFreeTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isFree)
        {
            return isFree ? "Free" : "$0.00";
        }
        return "Free";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Gets the first letter of a string
/// </summary>
public class FirstLetterConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
        {
            return str[0].ToString().ToUpper();
        }
        return "?";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a percentage to a width based on parameter
/// </summary>
public class PercentToWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double percent)
        {
            double maxWidth = 700;
            if (parameter is string paramStr && double.TryParse(paramStr, out var parsed))
            {
                maxWidth = parsed;
            }
            else if (parameter is double paramDouble)
            {
                maxWidth = paramDouble;
            }

            return Math.Max(0, Math.Min(maxWidth, percent / 100 * maxWidth));
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts an integer to an index (for ComboBox binding)
/// </summary>
public class IntToIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue - 1; // Convert 1-based to 0-based index
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index + 1; // Convert 0-based index to 1-based
        }
        return 1;
    }
}

/// <summary>
/// Converts a boolean (IsEditMode) to appropriate save button text
/// </summary>
public class BoolToSaveTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? "Update Application" : "Create Application";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a string path to visibility - visible if path exists and is not empty
/// </summary>
public class PathToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrWhiteSpace(path))
        {
            // Check if file exists for local paths
            if (!path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return System.IO.File.Exists(path)
                    ? System.Windows.Visibility.Visible
                    : System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Visible;
        }
        return System.Windows.Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Inverse of PathToVisibilityConverter - visible if path is empty or doesn't exist
/// </summary>
public class InversePathToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string path && !string.IsNullOrWhiteSpace(path))
        {
            // Check if file exists for local paths
            if (!path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return System.IO.File.Exists(path)
                    ? System.Windows.Visibility.Collapsed
                    : System.Windows.Visibility.Visible;
            }
            return System.Windows.Visibility.Collapsed;
        }
        return System.Windows.Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
