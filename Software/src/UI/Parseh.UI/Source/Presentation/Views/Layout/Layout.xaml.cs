namespace Parseh.UI.Views;

public partial class Layout : ContentLayout
{
    LayoutViewModel _model = default!;

    public Layout() => Init();

    private void Init()
    {
        _model = new LayoutViewModel(this);
        InitializeComponent();
    }

    private void OnAtcivated(object sender, EventArgs e) => _model.LayoutActivationCommand.Execute(null);

    private void OnDeactivated(object sender, EventArgs e) => _model.LayoutActivationCommand.Execute(null);
}