namespace Parseh.UI.Views;

using System.Windows.Data;
using System.Windows.Markup;

public abstract class BindConverter<TOwner> : MarkupExtension, IValueConverter
    where TOwner : BindConverter<TOwner>, new()
{
    public static TOwner Default = new();

    public override object ProvideValue(IServiceProvider serviceProvider) => Default ?? (Default = new());

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
}