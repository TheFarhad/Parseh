namespace Parseh.UI.Views;

using System.Security;

public partial class Signin : ContentPage<SigninViewModel>, IHavePassword
{
    public SecureString SecurePasscode => Passcode.SecurePassword;

    public Signin() => InitializeComponent();
}