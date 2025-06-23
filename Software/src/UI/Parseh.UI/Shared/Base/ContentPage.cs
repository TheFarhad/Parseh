namespace Parseh.UI.Views;

public class ContentPage : Page
{
    const double FadeFrom = 0;
    const double FadeTo = 1;
    public const double AnimateDuration = 0.4;
    public bool ShouldUnload { get; set; } = false;
    public readonly Ioc Ioc = Ioc.Default;

    public ContentPage() => Init();

    public virtual async Task AnimateIn()
    {
        var offset = this.WindowWidth;
        await this.TranlateFadeAsync(new(offset, 0, -offset, 0), new(0), FadeFrom, FadeTo, AnimateDuration);
    }

    public virtual async Task AnimateOut()
    {
        var offset = this.WindowWidth;
        await this.TranlateFadeAsync(new(0), new(-offset, 0, offset, 0), FadeTo, FadeFrom, AnimateDuration);
    }

    #region Private Functionality

    void Init() => Loaded += OnLoaded;

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ShouldUnload) App.DispatchAsync(AnimateOut);
        else
        {
            Visibility = Visibility.Collapsed;
            App.DispatchAsync(AnimateIn);
        }

        /* Why Dispatcher?
         چون یوآی برنامه در ترد اصلی اجرا می شود
         و ما میخواهیم انیمشین روی این پیج که یک المان یوآی است در یک ترد دیر انجام شود
         و از آنجا که این کار یوآی را تحت تاثیر قرار میدهد
         پس برای اینکه این متود بتواند در یک تردی متفاوت از ترد اصلی اجرا شود و با یوآی تعامل داشته باشد
         آن را توسط دیسپچر اجرا میکنیم
         */
    }

    #endregion
}

public class ContentPage<TVM> : ContentPage where TVM : VM, new()
{
    public readonly TVM Model = default!;

    public ContentPage() => DataContext = Model = Ioc.Service<TVM>() ?? new();
}