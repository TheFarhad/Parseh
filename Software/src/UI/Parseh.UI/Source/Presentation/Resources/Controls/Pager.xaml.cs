namespace Parseh.UI.Views;

public sealed partial class Pager : Component
{
    public static DependencyProperty PageProperty =
        DependencyProperty
        .Register(nameof(Page),
        typeof(ContentPage),
        typeof(Pager),
        new UIPropertyMetadata(default(ContentPage), OnPageChanged, null));

    public ContentPage Page
    {
        get => GetValue(PageProperty).As<ContentPage>();
        set => SetValue(PageProperty, value);
    }

    public Pager() => Init();

    #region Private Functionality

    void Init()
    {
        InitializeComponent();

        //if (DesignerProperties.GetIsInDesignMode(this))
        //    Current.Content = Generic.Self.ViewModel.Page;
    }

    static void OnPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var pager = sender.As<Pager>();
        var currentpage = e.NewValue.As<ContentPage>();

        var prevFrame = pager.Previous;
        var currFrame = pager.Current;

        var prevPage = currFrame.Content;
        currFrame.Content = null;

        if (prevPage is ContentPage oldpage)
        {
            // در اینجا هم تعیین میکنیم که در هنگام لود مجدد، باید انیمیت اوت اتفاق بیفتد
            oldpage.ShouldUnload = true;
            prevFrame.Content = oldpage;

            // در اینجا م به اندازه مدت زمان اجرای انیمشین صبر میکنیم تا به صورت کامل اجرا شود
            // سپس کنترل پشتی را نال میکنیم تا پیج قبلی کاملا از مموری حذف شود
            Task.Delay(TimeSpan.FromSeconds(ContentPage.AnimateDuration))
                .ContinueWith((t) =>
                {
                    App.Dispatch(() => oldpage.Content = null);
                });
        }
        else
        {
            // در اینجا چون پیج جاری را به المان پشتی اختصاص میدهیم
            // پس این پیج، مجددا بارگذاری شده و بنابراین متود لود آن فراخوانی میشود
            prevFrame.Content = prevPage;
        }
        currFrame.Content = currentpage;
    }

    static object OnPageChanging(DependencyObject sender, object newpage)
    {
        var pager = sender.As<Pager>();
        var currentpage = newpage.As<ContentPage>();

        var prevFrame = pager.Previous;
        var currFrame = pager.Current;

        var prevPage = currFrame.Content;
        currFrame.Content = null;

        if (prevPage is ContentPage oldpage)
        {
            // در اینجا هم تعیین میکنیم که در هنگام لود مجدد، باید انیمیت اوت اتفاق بیفتد
            oldpage.ShouldUnload = true;
            prevFrame.Content = oldpage;

            // در اینجا م به اندازه مدت زمان اجرای انیمشین صبر میکنیم تا به صورت کامل اجرا شود
            // سپس کنترل پشتی را نال میکنیم تا پیج قبلی کاملا از مموری حذف شود
            Task.Delay(TimeSpan.FromSeconds(ContentPage.AnimateDuration))
                .ContinueWith((t) =>
                {
                    App.Dispatch(() => oldpage.Content = null);
                });
        }
        else
        {
            // در اینجا چون پیج جاری را به المان پشتی اختصاص میدهیم
            // پس این پیج، مجددا بارگذاری شده و بنابراین متود لود آن فراخوانی میشود
            prevFrame.Content = prevPage;
        }
        currFrame.Content = currentpage;
        return currentpage;
    }

    #endregion
}
