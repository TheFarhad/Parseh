using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;

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

internal sealed class PopupMenuIconTypeToImageSource : BindConverter<PopupMenuIconTypeToImageSource>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value.As<string>().IsEmpty())
            return null!;

        return new BitmapImage(new Uri($"/Source/Presentation/Resources/Images/Icon/{value.As<string>()}.png", uriKind: UriKind.Relative));
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

internal sealed class PopupMenuItemTypeToVisibility : BindConverter<PopupMenuItemTypeToVisibility>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = Visibility.Collapsed;

        if (parameter is null)
            return result;

        if (!Enum.TryParse(parameter.As<String>(), out PopupMenuItemType desiredType))
            return result;


        value.Is<PopupMenuItemType>(type =>
        {
            result = type == desiredType ? Visibility.Visible : Visibility.Collapsed;
        });

        return result;
    }
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

internal sealed class MessageTypeToIcon : BindConverter<MessageTypeToIcon>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        BitmapImage result = default!;
        if (value is null)
            return null!;

        value.Is<MessageType>(type =>
        {
            result = type switch
            {
                MessageType.Information => ToIcon("Information"),
                MessageType.Warning => ToIcon("Warning"),
                MessageType.Error => ToIcon("Error"),
                _ => null!
            };
        });
        return result;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    BitmapImage ToIcon(string icon)
        => new BitmapImage(new Uri($"/Source/Presentation/Resources/Images/Icon/{icon}.png", uriKind: UriKind.Relative));
}

internal sealed class MessageTypeToTitle : BindConverter<MessageTypeToTitle>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var result = "Information";
        value?.Is<MessageType>(type => result = type.ToString());
        return result;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}