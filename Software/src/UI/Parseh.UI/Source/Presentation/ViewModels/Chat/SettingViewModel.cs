namespace Parseh.UI.ViewModels;

public sealed class SettingViewModel : VM
{
    #region Properties

    public double CornerRadius { get => Get(); private set => Set(value); }

    #endregion

    #region Properties


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

    }

    #endregion

    #region Command Methods


    #endregion
}