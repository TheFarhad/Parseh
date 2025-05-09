namespace Parseh.UI.Views;

public class ContentPage : Page
{
    const double FadeFrom = 0;
    const double FadeTo = 1;
    public const double AnimateDuration = 0.5;
    public bool ShouldUnload { get; set; } = false;

    public ContentPage() => Init();

    public virtual async Task AnimateIn()
    {
        var size = this.WindowWidth;
        await this.TranlateFadeAsync(new(size, 0, -size, 0), new(0), FadeFrom, FadeTo, AnimateDuration);
    }

    public virtual async Task AnimateOut()
    {
        var size = this.WindowWidth;
        await this.TranlateFadeAsync(new(0), new(-size, 0, size, 0), FadeTo, FadeFrom, AnimateDuration);
    }

    #region Private Functionality

    void Init() => Loaded += OnLoaded;

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (ShouldUnload) App.Dispatch(AnimateOut);
        else
        {
            Visibility = Visibility.Collapsed;
            App.Dispatch(AnimateIn);
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
    protected readonly NetCoreIoC Ioc = NetCoreIoC.Self;

    public ContentPage() => DataContext = Model = Ioc.Get<TVM>() ?? new();
}