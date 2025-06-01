namespace Parseh.UI.ViewModels;

public sealed class SettingMenuViewModel : VM
{
    #region Properties

    public double CornerRadius { get => Get(); private set => Set(value); }
    public string Title { get => Get(); private set => Set(value); }

    #endregion

    #region Properties


    #endregion

    public SettingMenuViewModel() => Init();

    #region Private Functionality

    protected override void OnCreate() => base.OnCreate();

    void Init()
    {
        InitProperties();
        InitCommands();
    }

    void InitProperties()
    {
        Title = "Settings";
        CornerRadius = Constant.CornerRadius;
    }

    void InitCommands()
    {

    }

    #endregion

    #region Command Methods


    #endregion
}