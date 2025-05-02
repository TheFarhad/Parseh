namespace Parseh.UI.BindingConverters;

internal sealed class Pager : BindConverter<Pager>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<PageMode>() switch
        {
            PageMode.Signin => new Signin(),
            PageMode.Signup => new Signup(),
            PageMode.Chat => new Chat(),
            _ => new Signin()
        };

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

internal sealed class BooleanToVisibility : BindConverter<BooleanToVisibility>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<bool>() ? Visibility.Visible : Visibility.Collapsed;

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<Visibility>() == Visibility.Visible;
}

internal sealed class BooleanToVisibilityRevert : BindConverter<BooleanToVisibilityRevert>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<bool>() ? Visibility.Collapsed : Visibility.Visible;

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<Visibility>() != Visibility.Visible;
}
