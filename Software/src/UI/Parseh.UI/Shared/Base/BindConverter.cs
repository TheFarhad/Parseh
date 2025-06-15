namespace Parseh.UI.Views;

using System.Windows.Data;
using System.Windows.Markup;

public abstract class BindConverter<TOwner> : MarkupExtension, IValueConverter
    where TOwner : BindConverter<TOwner>, new()
{
    static readonly Lazy<TOwner> _lazyInstance = new Lazy<TOwner>(() => new());
    public static TOwner Default { get; } = _lazyInstance.Value;

    protected BindConverter() { }

    public override object ProvideValue(IServiceProvider serviceProvider) => Default;

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
    public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
}