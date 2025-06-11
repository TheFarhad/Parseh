namespace Parseh.UI.Views;

public partial class Passcode : Component
{
    public Passcode() => Init();

    void Init()
    {
        InitializeComponent();
    }

    void OnCurrentPasscodeChanged(object sender, RoutedEventArgs e)
        => DataContext?.Is<PasscodeViewModel>(passcodevm => passcodevm.Password = PasscodeBox.SecurePassword);

    void OnNewPasscodeChanged(object sender, RoutedEventArgs e)
        => DataContext?.Is<PasscodeViewModel>(passcodevm => passcodevm.New = New.SecurePassword);

    void OnConfirmPasscodeChanged(object sender, RoutedEventArgs e)
        => DataContext?.Is<PasscodeViewModel>(passcodevm => passcodevm.Confirm = Confirm.SecurePassword);
}
