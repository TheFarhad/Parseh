using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Parseh.UI.ViewModels;

internal sealed class LayoutViewModel : VM
{
    const string IconPath = "/Source/Presentation/Resources/Images/Icon/";
    Layout _layout = default!;
    WindowState _state => _layout.WindowState;
    LayoutFitAdjuster _fitAdjuster = default!;
    WindowDockPosition _dockPosition = WindowDockPosition.Undocked;

    public double MinWidth { get => Get(); private set => Set(value); }
    public double MinHeight { get => Get(); private set => Set(value); }
    //TODO: آیا پرایپوت ست مشکلی در بایندینگ دو طرفه ایجاد نمیکند
    public double Width { get => Get(); private set => Set(value); }
    public double Height { get => Get(); private set => Set(value); }
    public bool ActivateMode { get => Get(); private set => Set(value); }
    public double CaptionHeight { get => Get(); private set => Set(value); }
    public double CornerRadius { get => NormalState() ? Get() : 0; private set => Set(value); }
    public Thickness InnerContentPadding { get => Get(); private set => Set(value); }
    public Thickness OuterMargin { get => NormalState() ? Get() : new Thickness(0); private set => Set(value); }
    public Thickness ResizeBorderThickness { get => NormalState() ? Get() : new Thickness(0); private set => Set(value); }
    public Thickness OuterBorderThickness { get => NormalState() ? Get() : new Thickness(1); private set => Set(value); }
    public ImageSource RestoreButtonImage
    {
        get => NormalState() ? Get() : ImageSource("Restore");
        private set => Set(value);
    }

    public IRelayCommand MinimizeCommand { get; private set; } = default!;
    public IRelayCommand RestoreCommand { get; private set; } = default!;
    public IRelayCommand CloseCommand { get; private set; } = default!;
    public IRelayCommand CaptionManuCommand { get; private set; } = default!;

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
        Width = 900;
        Height = 506;
        ActivateMode = true;
        CaptionHeight = 27;
        CornerRadius = 6;
        OuterMargin = new(3);
        InnerContentPadding = new(0);
        OuterBorderThickness = new(0);
        ResizeBorderThickness = new(3);
        RestoreButtonImage = ImageSource("Maximize");
    }

    void InitLayout(Layout layout)
    {
        _layout = layout;
        _layout.DataContext = this;

        _fitAdjuster = new LayoutFitAdjuster(layout);
        _fitAdjuster.WindowDockChanged += WindowDockChanged;


        _layout.StateChanged += OnStateChanged;
        _layout.Activated += OnActivated;
        _layout.Deactivated += OnDeactivated;
    }

    void InitCommands()
    {
        MinimizeCommand = new Command(OnMinimize);
        RestoreCommand = new Command(OnRestore);
        CloseCommand = new Command(OnClose);
        CaptionManuCommand = new Command(OnCaptionManu);
    }

    void OnStateChanged(object? sender, EventArgs e) => OnNotifyStateChanged();
    private void WindowDockChanged(WindowDockPosition dockPosition)
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
        Notify(nameof(ResizeBorderThickness));
        Notify(nameof(RestoreButtonImage));
    }

    BitmapImage ImageSource(string icon)
        => new BitmapImage(new Uri($"/Source/Presentation/Resources/Images/Icon/{icon}.png", uriKind: UriKind.Relative));

    #endregion

    #region Commands

    void OnMinimize() => _layout.WindowState = WindowState.Minimized;
    void OnRestore() => _layout.WindowState ^= WindowState.Maximized;
    void OnClose() => _layout.Close();
    void OnCaptionManu() => SystemCommands.ShowSystemMenu(_layout, _layout.PointToScreen(new Point(30, 5)));

    #endregion
}
