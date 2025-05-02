namespace Parseh.UI.ViewModels;

public sealed class SigninViewModel : VM
{
    public string Username { get => Get(); set => Set(value); }
    public string Passcode { get => Get(); set => Set(value); }

    public ICommand SigninCommand { get; private set; } = default!;
    public ICommand SignupCommand { get; private set; } = default!;

    public SigninViewModel() => Init();

    #region Private Functionality

    protected override void OnCreate() => base.OnCreate();

    void Init()
    {
        InitProperties();
        InitCommands();
    }

    void InitProperties() { }

    void InitCommands()
    {
        SigninCommand = new Command(async () => await Signin());
        SignupCommand = new Command(async () => await Signup());
    }

    #endregion

    #region Commands

    async Task Signin() { }
    async Task Signup() { }

    #endregion

}
