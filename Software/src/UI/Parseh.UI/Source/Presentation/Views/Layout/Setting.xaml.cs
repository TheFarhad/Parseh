namespace Parseh.UI.Views;

public partial class Setting : ContentLayout
{
    SettingViewModel _model = default!;

    public Setting() => Init();

    void Init()
    {
        _model = new SettingViewModel(this);
        InitializeComponent();
    }
}
