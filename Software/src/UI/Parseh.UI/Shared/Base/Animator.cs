namespace Parseh.UI;

using System.Windows.Media;
using System.Windows.Media.Animation;

internal static class Animator
{
    // TODO: برای حالت های چب به راست، راست به چپ، بالا به پایین و پایین به بالا هم متود پیشفرض نوشته شود
    const double DecelerationRatio = 0.5;
    const double Duration = 0.5;

    public static async Task TranlateFadeAsync(this FrameworkElement element, Thickness translateFrom, Thickness trablateTo, double fadeFrom, double fadeTo, double duration = Duration)
    {
        await OnAnimateAsync(element, sb =>
        {
            Translate(sb, translateFrom, trablateTo, duration);
            FadeTo(sb, fadeFrom, fadeTo, duration);
        }, duration);
    }

    public static async Task FadeToAsync(this FrameworkElement element, double from = 0, double to = 1, double duration = Duration)
        => await OnAnimateAsync(element, sb => FadeTo(sb, from, to, duration), duration);

    public static async Task TranslateAsync(this FrameworkElement element, Thickness from, Thickness to, double duration = Duration)
        => await OnAnimateAsync(element, sb => Translate(sb, from, to, duration), duration);

    public static async Task RotateTo(this FrameworkElement element, double from, double to, double duration = Duration)
        => await OnAnimateAsync(element, sb => RotateTo(sb, element, from, to, duration), duration);

    #region Private Functionality

    static void Translate(this Storyboard sb, Thickness from, Thickness to, double duration = Duration)
    {
        var animation = new ThicknessAnimation
        {
            DecelerationRatio = DecelerationRatio,
            Duration = new(TimeSpan.FromSeconds(duration)),
            From = from,
            To = to
        };
        Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));
        sb.Children.Add(animation);
    }

    static void RotateTo(this Storyboard sb, FrameworkElement element, double from, double to, double duration = Duration)
    {
        element.RenderTransformOrigin = new(0.5, 0.5);
        element.RenderTransform = new RotateTransform();
        var animation = new DoubleAnimation
        {
            Duration = TimeSpan.FromSeconds(duration),
            From = from,
            To = to,
            DecelerationRatio = DecelerationRatio
        };
        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
        sb.Children.Add(animation);
    }

    static void FadeTo(this Storyboard sb, double from = 0, double to = 1, double duration = Duration)
    {
        var animation = new DoubleAnimation
        {
            Duration = TimeSpan.FromSeconds(duration),
            From = from,
            To = to,
            DecelerationRatio = DecelerationRatio
        };
        Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
        sb.Children.Add(animation);
    }

    static void ScaleTo(this Storyboard sb, FrameworkElement element, double from, double to, double duration = Duration)
    {
        element.RenderTransformOrigin = new(0.5, 0.5);
        element.RenderTransform = new ScaleTransform();

        var time = TimeSpan.FromSeconds(duration);
        var scalex = new DoubleAnimation
        {
            Duration = time,
            From = from,
            To = to,
            DecelerationRatio = DecelerationRatio
        };
        Storyboard.SetTargetProperty(scalex, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));

        var scaley = new DoubleAnimation
        {
            Duration = time,
            From = from,
            To = to,
            DecelerationRatio = DecelerationRatio
        };
        Storyboard.SetTargetProperty(scaley, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));

        sb.Children.Add(scalex);
        sb.Children.Add(scaley);
    }

    static async Task OnAnimateAsync(FrameworkElement element, Action<Storyboard> animations, double duration = Duration)
    {
        var board = new Storyboard();
        animations?.Invoke(board);
        board.Begin(element);
        element.Visibility = Visibility.Visible;
        // TODO: کاربرد ؟؟
        // صفر هم باشد تغییری نمیکند
        await Task.Delay(TimeSpan.FromSeconds(duration));
    }

    #endregion
}

// TODO: پروژه لوک مالپاس بررسی شود که دقیقا کجاها بعد از اعمال انیمشین، ویزیبیلیتی کولاپس شده و به چه دلیلی

