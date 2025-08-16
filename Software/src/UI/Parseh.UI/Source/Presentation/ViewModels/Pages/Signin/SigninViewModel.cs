using Parseh.UI.Source.Infra.ExternalApi;

namespace Parseh.UI.ViewModels;

public sealed class SigninViewModel : VM
{
    private readonly UserService _userService;

    public string Username
    {
        get => Get();
        set
        {
            Set(value);
            Notify(SigninCommand);
        }
    }
    //public SecureString Passcode { get => Get(); private set => Set(value); }
    public string Passcode { get => Get(); private set => Set(value); }
    public bool IsSigning
    {
        get => Get();
        set
        {
            Set(value);
            Notify(SigninCommand);
        }
    }

    public IRelayCommand SigninCommand { get; private set; } = default!;
    public IRelayCommand SignupCommand { get; private set; } = default!;
    public IRelayCommand SignoutCommand { get; private set; } = default!;

    public SigninViewModel(/*UserService userService*/)
    {
        Init();

        // TODO:
        //_userService = userService;
    }

    protected override void OnCreate() { }

    #region Private Functionality

    private void Init()
    {
        InitProperties();
        InitCommands();
    }

    private void InitProperties()
    {
        Username = Empty;
        Passcode = Empty;
        IsSigning = false;
    }

    private void InitCommands()
    {
        // TODO: شرط اجرایی کامند برگردانه شود
        SigninCommand = new Command<IHavePassword>(async page => await Signin(page)/*, _ => Username.Length > 0 && !IsSigning*/);
        SignupCommand = new Command(Signup);
        SignoutCommand = new Command(async () => await Signout());
    }

    #endregion

    #region Commands

    private async Task Signin(IHavePassword signinPage)
    {
        await RunAsync(async () =>
        {
            IsSigning = true;

            Passcode = signinPage.SecurePasscode.Unsecure();

            await Task.Delay(20);

            Cortex.Default.Model.ToPage(PageMode.Chat);
            // TODO:
            // get user-data from server and init setting-viewmodel
            //Cortex.Default.Model.SettingMenuModel = ;

            IsSigning = false;
        });
    }

    private void Signup()
        => Cortex.Default.Model.ToPage(PageMode.Signup);

    private async Task Signout()
    {
        await RunAsync(async () =>
        {
            // TODO: Remove JWT
            // TODO: Remove Setting Options

            await Task.Delay(1000);
        });
    }

    #endregion
}
