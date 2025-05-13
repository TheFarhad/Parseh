namespace Parseh.UI.Views;

public partial class Layout : ContentLayout
{
    LayoutViewModel _model = default!;

    public Layout() => Init();

    void Init()
    {
        _model = new LayoutViewModel(this);
        InitializeComponent();
    }
}