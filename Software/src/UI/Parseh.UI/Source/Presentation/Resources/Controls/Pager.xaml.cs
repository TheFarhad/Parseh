namespace Parseh.UI.Views;

public sealed partial class Pager : Component
{
    public static DependencyProperty PageProperty =
        DependencyProperty
        .Register(nameof(Page),
        typeof(ContentPage),
        typeof(Pager),
        new UIPropertyMetadata(default(ContentPage), null, OnPageChanging));

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

    static object OnPageChanging(DependencyObject sender, object newpage)
    {
        var pager = sender.As<Pager>();
        var currentpage = newpage.As<ContentPage>();

        var prevControl = pager.Previous;
        var currControl = pager.Current;

        var oldPageContent = currControl.Content;
        currControl.Content = null;

        if (oldPageContent is ContentPage oldpage)
        {
            // در اینجا هم تعیین میکنیم که در هنگام لود مجدد، باید انیمیت اوت اتفاق بیفتد
            oldpage.ShouldUnload = true;
            prevControl.Content = oldpage;

            // در اینجا م به اندازه مدت زمان اجرای انیمشین صبر میکنیم تا به صورت کامل اجرا شود
            // سپس کنترل پشتی را نال میکنیم تا پیج قبلی کاملا از مموری حذف شود
            Task
                .Delay(TimeSpan.FromSeconds(ContentPage.AnimateDuration))
                .ContinueWith((t) =>
                {
                    App.Dispatch(() => oldpage.Content = null);
                });
        }
        else
        {
            // در اینجا چون پیج جاری را به المان پشتی اختصاص میدهیم
            // پس این پیج، مجددا بارگذاری شده و بنابراین متود لود آن فراخوانی میشود
            prevControl.Content = oldPageContent;
        }

        currControl.Content = currentpage;
        return currentpage;
    }

    #endregion
}
