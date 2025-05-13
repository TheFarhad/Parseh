namespace Parseh.UI;

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
            Fade(sb, fadeFrom, fadeTo, duration);
        }, duration);
    }

    public static async Task FadeAsync(this FrameworkElement element, double from = 0, double to = 1, double duration = Duration)
        => await OnAnimateAsync(element, sb => Fade(sb, from, to, duration), duration);

    public static async Task TranslateAsync(this FrameworkElement element, Thickness from, Thickness to, double duration = Duration)
        => await OnAnimateAsync(element, sb => Translate(sb, from, to, duration), duration);

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

    static void Fade(this Storyboard sb, double from = 0, double to = 1, double duration = Duration)
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


