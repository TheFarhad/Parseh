namespace Parseh.UI.ViewModels;

public sealed class SettingViewModel : VM
{
    #region Properties

    public double CornerRadius { get => Get(); private set => Set(value); }

    #endregion

    #region Properties

    public IRelayCommand CloseCommand { get; private set; } = default!;
    public IRelayCommand CloseSettingMenuCommand { get; private set; } = default!;

    #endregion

    public SettingViewModel() => Init();

    #region Private Functionality

    protected override void OnCreate() => base.OnCreate();

    void Init()
    {
        InitProperties();
        InitCommands();
    }

    void InitProperties()
    {
        CornerRadius = Constant.CornerRadius;
    }

    void InitCommands()
    {
        CloseCommand = new Command(Close);
        CloseSettingMenuCommand = new Command(CloseSettingMenu);
    }

    #endregion

    #region Command Methods

    void Close() => Generic.Default.Model.IsOpenSettingMenu = false;
    void CloseSettingMenu() => Generic.Default.Model.IsOpenSettingMenu = false;

    #endregion
}