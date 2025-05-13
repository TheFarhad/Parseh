namespace Parseh.UI.ViewModels;

public sealed class SigninViewModel : VM
{
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

    public SigninViewModel() => Init();

    #region Private Functionality

    protected override void OnCreate() { }

    void Init()
    {
        InitProperties();
        InitCommands();
    }

    void InitProperties()
    {
        Username = Empty;
        Passcode = Empty;
        IsSigning = false;
    }

    void InitCommands()
    {
        // TODO: شرط اجرایی کامند برگردانه شود
        SigninCommand = new Command<IHavePassword>(async page => await Signin(page)/*, _ => Username.Length > 0 && !IsSigning*/);
        SignupCommand = new Command(Signup);
        SignoutCommand = new Command(async () => await Signout());
    }

    #endregion

    #region Commands

    async Task Signin(IHavePassword signinPage)
    {
        await RunAsync(async () =>
        {
            IsSigning = true;

            Passcode = signinPage.SecurePasscode.Unsecure();

            await Task.Delay(20);
            Generic.Default.Model.ToPage(PageMode.Chat);

            IsSigning = false;
        });
    }
    void Signup() => Generic.Default.Model.ToPage(PageMode.Signup);

    async Task Signout()
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
