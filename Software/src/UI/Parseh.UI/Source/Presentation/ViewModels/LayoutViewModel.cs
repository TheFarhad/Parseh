namespace Parseh.ViewModels;

internal sealed class LayoutViewModel : VM
{
    private readonly ParsehLayout _layout = default!;

    public int MinWidth { get; } = 700;
    public int MinHeight { get; } = 500;
    public int Width { get; } = 900;
    public int Height { get; } = 600;
    public bool OwnerActivate { get; private set; } = false;

    internal LayoutViewModel(ParsehLayout layout)
    {
        _layout = layout;
        Init();
    }

    void Init()
    {
        //_layout.DataContext = this;

        _layout.StateChanged += OnStateChangd;
        _layout.Activated += OnActivated;
        _layout.Deactivated += OnDeactivated;
    }

    void OnStateChangd(object? sender, EventArgs e)
    {

    }

    void OnActivated(object? sender, EventArgs e) => OwnerActivate = true;
    void OnDeactivated(object? sender, EventArgs e) => OwnerActivate = false;
}
