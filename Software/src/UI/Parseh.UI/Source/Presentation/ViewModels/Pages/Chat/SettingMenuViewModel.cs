namespace Parseh.UI.ViewModels;

public sealed class SettingMenuViewModel : ViewModel
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

    public SettingMenuViewModel()
        => Init();

    public void ClearUserdata()
    {
        Name = new();
        Username = new();
        Passcode = new();
        Email = new();
    }

    #region Private Functionality

    protected override void OnCreate()
        => base.OnCreate();

    private void Init()
    {
        InitProperties();
        InitCommands();
    }

    private void InitProperties()
    {
        // TODO: این مقادیر باید از دیتابیس یا سرویس کاربر خوانده شوند
        //       فقط اگر لازم بود مقدار دیفالت داده شود
        Title = "Settings";
        CornerRadius = Constant.CornerRadius;
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

    private void InitCommands()
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
        App.Cortex.ToPage(PageMode.Signin);
    }



    #endregion
}