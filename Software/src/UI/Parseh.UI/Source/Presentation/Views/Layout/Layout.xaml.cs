namespace Parseh.UI.Views;

public partial class Layout : AppLayout
{
    LayoutViewModel _viewmodel = default!;

    public Layout() => Init();

    void Init()
    {
        _viewmodel = new LayoutViewModel(this);
        InitializeComponent();
    }
}