using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FluxRoute.Converters;

public sealed class NullToVisibilityConverter : IValueConverter
{
    public Visibility NullValue { get; set; } = Visibility.Collapsed;
    public Visibility NotNullValue { get; set; } = Visibility.Visible;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is not null ? NotNullValue : NullValue;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
