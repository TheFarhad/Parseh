namespace Parseh.UI.ViewModels;

using System.Windows.Media;
using System.Windows.Media.Imaging;

internal sealed class LayoutViewModel : VM
{
    #region Fileds

    Layout _view = default!;
    WindowState _state => _view.WindowState;
    LayoutFitAdjuster _fitAdjuster = default!;
    // TODO: ببین در پروژه اصلی با پراپرتی زیر دقیقا چه کاری کرده
    WindowDockPosition _dockPosition = WindowDockPosition.Undocked;

    #endregion

    #region Properties

    public double MinWidth { get => Get(); private set => Set(value); }
    public double MinHeight { get => Get(); private set => Set(value); }
    //TODO: آیا پرایپوت ست مشکلی در بایندینگ دو طرفه ایجاد نمیکند
    public double Width { get => Get(); private set => Set(value); }
    public double Height { get => Get(); private set => Set(value); }
    public bool ActivateMode { get => Get(); private set => Set(value); }
    public double CaptionHeight { get => Get(); private set => Set(value); }
    public double CornerRadius { get => NormalState() ? Get() : 0; private set => Set(value); }
    public bool IsLayoutActivated { get => Get(); private set => Set(value); }
    public Thickness InnerContentPadding { get => Get(); private set => Set(value); }
    public Thickness OuterMargin { get => NormalState() ? Get() : new Thickness(0); private set => Set(value); }
    public Thickness ResizeBorderThickness { get => NormalState() ? Get() : new Thickness(0); private set => Set(value); }
    public Thickness OuterBorderThickness { get => NormalState() ? Get() : new Thickness(1); private set => Set(value); }
    public ImageSource RestoreButtonImage
    {
        get => NormalState() ? Get() : ImageSource("Restore");
        private set => Set(value);
    }

    #endregion

    #region Commands

    public IRelayCommand MinimizeCommand { get; private set; } = default!;
    public IRelayCommand RestoreCommand { get; private set; } = default!;
    public IRelayCommand CloseCommand { get; private set; } = default!;
    public IRelayCommand CaptionMenuCommand { get; private set; } = default!;
    public IRelayCommand LayoutActivationCommand { get; private set; } = default!;

    #endregion

    internal LayoutViewModel(Layout layout) => Init(layout);

    #region Private Functionality

    protected override void OnCreate() => base.OnCreate();

    void Init(Layout layout)
    {
        InitProperties();
        InitLayout(layout);
        InitCommands();
    }

    void InitProperties()
    {
        MinWidth = 700;
        MinHeight = 394;
        // TODO:
        //Width = 900;
        //Height = 506;
        Width = 1068;
        Height = 600;
        ActivateMode = true;
        CaptionHeight = 27;
        CornerRadius = 6;
        OuterMargin = new(3);
        IsLayoutActivated = false;
        InnerContentPadding = new(0);
        // TODO: اگر صفر باشد، نمی توان ویندو را ریسیز کرد
        // بوردر راش هم نیاید ترنسپزنت باشد
        // چرا؟
        OuterBorderThickness = new(0);
        ResizeBorderThickness = new(3);
        RestoreButtonImage = ImageSource("Maximize");
    }

    void InitLayout(Layout layout)
    {
        _view = layout;
        _view.DataContext = this;

        _fitAdjuster = new LayoutFitAdjuster(layout);
        _fitAdjuster.WindowDockChanged += WindowDockChanged;

        _view.StateChanged += OnStateChanged;
        _view.Activated += OnActivated;
        _view.Deactivated += OnDeactivated;
    }

    void InitCommands()
    {
        MinimizeCommand = new Command(OnMinimize);
        RestoreCommand = new Command(OnRestore);
        CloseCommand = new Command(OnClose);
        CaptionMenuCommand = new Command(OnCaptionMenu);
        LayoutActivationCommand = new Command(() => IsLayoutActivated ^= true);
    }

    void OnStateChanged(object? sender, EventArgs e) => OnNotifyStateChanged();
    void WindowDockChanged(WindowDockPosition dockPosition)
    {
        _dockPosition = dockPosition;
        OnNotifyStateChanged();
    }
    void OnActivated(object? sender, EventArgs e) => ActivateMode = true;
    void OnDeactivated(object? sender, EventArgs e) => ActivateMode = false;

    bool NormalState() => _state == WindowState.Normal;
    void OnNotifyStateChanged()
    {
        Notify(nameof(CornerRadius));
        Notify(nameof(OuterMargin));
        Notify(nameof(OuterBorderThickness));
        Notify(nameof(ResizeBorderThickness));
        Notify(nameof(RestoreButtonImage));
    }

    BitmapImage ImageSource(string icon)
        => new(new Uri($"/Source/Presentation/Resources/Images/Icon/{icon}.png", uriKind: UriKind.Relative));

    #endregion

    #region Command Methods

    void OnMinimize() => _view.WindowState = WindowState.Minimized;
    void OnRestore() => _view.WindowState ^= WindowState.Maximized;
    void OnClose() => _view.Close();
    void OnCaptionMenu() => SystemCommands.ShowSystemMenu(_view, _view.PointToScreen(new Point(30, 5)));

    #endregion
}
