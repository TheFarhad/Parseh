namespace Parseh.UI.ViewModels;

public sealed class SettingMenuViewModel : VM
{
    #region Properties

    public double CornerRadius { get => Get(); private set => Set(value); }
    public string Title { get => Get(); private set => Set(value); }

    public EntryViewModel Name { get => Get(); set => Set(value); }
    public EntryViewModel Email { get => Get(); set => Set(value); }

    #endregion

    #region Commands


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
        Name = new()
        {
            Label = "Nickname",
            Text = "Farhad k"
        };
        Email = new()
        {
            Label = "Email",
            Text = "Farhadk.Dev@outlook.com"
        };
    }

    void InitCommands()
    {

    }

    #endregion

    #region Command Methods


    #endregion
}