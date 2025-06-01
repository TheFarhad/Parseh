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

    void OnAtcivated(object sender, EventArgs e) => _model.LayoutActivationCommand.Execute(null);

    void OnDeactivated(object sender, EventArgs e) => _model.LayoutActivationCommand.Execute(null);
}