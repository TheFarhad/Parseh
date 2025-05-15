namespace Parseh.UI.Views;

public partial class Setting : Component
{
    public Setting() => Init();

    void Init()
    {
        DataContext = NetIoC.Default.Get<SettingViewModel>();
        InitializeComponent();
    }
}
