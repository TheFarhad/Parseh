namespace Parseh.UI.BindingConverters;

using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

internal sealed class BooleanInvert : BindConverter<BooleanInvert>
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

internal sealed class BooleanToOpacity : BindConverter<BooleanToOpacity>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<bool>() ? 1 : 0;

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<double>() == 1 ? true : false;
}

internal sealed class BooleanToVisibilityInvert : BindConverter<BooleanToVisibilityInvert>
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

internal sealed class UnreadMessages : BindConverter<UnreadMessages>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.As<int>() <= 9 ? "9" : "9+";
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
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

        value.Is<DialogMessageType>(type =>
        {
            result = type switch
            {
                DialogMessageType.Information => ToIcon("Information"),
                DialogMessageType.Warning => ToIcon("Warning"),
                DialogMessageType.Error => ToIcon("Error"),
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
        value?.Is<DialogMessageType>(type => result = type.ToString());
        return result;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#region Message Bubble

internal sealed class ChatBubblePath : BindConverter<ChatBubblePath>
{
    Path _leftBubble = default!;
    Path _rightBubble = default!;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => ToPath(value.As<bool>());

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    Path ToPath(bool value)
    {
        // TODO: چرا فقط برای دو المان اول کار میکند؟؟


        // right bubble (send by me)
        if (value)
        {
            if (_rightBubble is null)
            {
                _rightBubble = new()
                {
                    StrokeThickness = 0,
                    Fill = App.Resource<Brush>("Gray14Brush"),
                    Data = Geometry.Parse("M 40 0 C 28 20 37 0 0 0 Z"),
                    Margin = new(0, -2, 6, 0),
                    HorizontalAlignment = HorizontalAlignment.Right
                };
            }
            return _rightBubble;
        }
        // left bubble (send by contact)
        else
        {
            if (_leftBubble is null)
            {
                _leftBubble = new()
                {
                    StrokeThickness = 0,
                    Fill = App.Resource<Brush>("Gray14Brush"),
                    Data = Geometry.Parse("M 0 0 C 9 20 1 0 40 0Z"),
                    Margin = new(6, -2, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left
                };
            }
            return _leftBubble;
        }
    }
}

internal sealed class ChatBubblePath1 : BindConverter<ChatBubblePath1>
{
    // TODO: چرا فقط برای دو المان اول کار میکند

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value.As<bool>() ? App.Resource<Path>("BubbleSendByMe") : App.Resource<Path>("BubbleSendByContact");

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#endregion