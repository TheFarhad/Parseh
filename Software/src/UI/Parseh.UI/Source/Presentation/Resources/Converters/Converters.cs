namespace Parseh.UI.BindingConverters;

internal sealed class RevertBoolean : BindConverter<RevertBoolean>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => !value.As<bool>();

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

internal sealed class BooleanToHAlignment : BindConverter<BooleanToHAlignment>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<bool>() ? HorizontalAlignment.Right : HorizontalAlignment.Left;

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<HorizontalAlignment>() == HorizontalAlignment.Right;
}

internal sealed class BooleanToHAlignmentRevert : BindConverter<BooleanToHAlignmentRevert>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<bool>() ? HorizontalAlignment.Left : HorizontalAlignment.Right;

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<HorizontalAlignment>() == HorizontalAlignment.Left;
}