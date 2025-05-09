namespace Parseh.UI.Views;

using System.Security;

internal interface IHavePassword
{
    SecureString SecurePasscode { get; }
}
