namespace Parseh.UI.Views;

public partial class Layout : ContentLayout
{
    LayoutViewModel _viewmodel = default!;

    public Layout(IServiceProvider provider) => Init(provider);

    void Init(IServiceProvider provider)
    {
        _viewmodel = new LayoutViewModel(this);
        InitializeComponent();
    }
}