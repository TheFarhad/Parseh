namespace Parseh.UI.Views;

using System.Windows.Data;
using System.Windows.Markup;

public abstract class BindConverter<TOwner> : MarkupExtension, IValueConverter
    where TOwner : BindConverter<TOwner>, new()
{
    public static TOwner Self = new();

    public override object ProvideValue(IServiceProvider serviceProvider) => Self ?? (Self = new());

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
}