namespace Parseh.UI.ViewModels;

public sealed class SettingMenuViewModel : VM
{
    #region Properties

    public double CornerRadius { get => Get(); private set => Set(value); }
    public string Title { get => Get(); private set => Set(value); }

    public EntryViewModel Name { get => Get(); set => Set(value); }
    public EntryViewModel Username { get => Get(); set => Set(value); }
    public PasscodeViewModel Passcode { get => Get(); set => Set(value); }
    public EntryViewModel Email { get => Get(); set => Set(value); }
    public string LogoutButton { get => Get(); private set => Set(value); }


    #endregion

    #region Commands

    public IRelayCommand LogoutCommand { get; private set; } = default!;
    public IRelayCommand ClearUserdataCommand { get; private set; } = default!;

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

        // TODO: remove form this and get userdata in signin-viewmodel after signin
        Name = new()
        {
            Label = "Nickname",
            Text = "Farhad k"
        };
        Username = new()
        {
            Label = "Username",
            Text = "Panah"
        };
        Passcode = new()
        {
            Label = "Password",
        };
        Email = new()
        {
            Label = "Email",
            Text = "Farhadk.Dev@outlook.com"
        };
        LogoutButton = "Logout";
    }

    void InitCommands()
    {
        LogoutCommand = new Command(async () => await Logout());
        ClearUserdataCommand = new Command(ClearUserdata);
    }

    #endregion

    #region Command Methods

    async Task Logout()
    {
        // TODO: نمایش یک دیالوگ باکس برای اطمینان از قصد کاربر برای خروج
        //TODO: Clear user data form cache


        await Task.Delay(1000);
        Cortex.Default.Model.ToPage(PageMode.Signin);
    }

    public void ClearUserdata()
    {
        Name = new();
        Username = new();
        Passcode = new();
        Email = new();
    }

    #endregion
}