namespace Parseh.UI.Resources;

internal sealed class PasscodeHasPlaceholder : Behavior<PasscodeHasPlaceholder, bool>
{
    protected override void OnValueChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        var passwordbox = uielement.As<PasswordBox>();
        var value = e.NewValue.As<bool>();

        if (passwordbox.IsNull()) return;

        passwordbox.PasswordChanged -= OnPasswordChanged;

        if (value) passwordbox.PasswordChanged += OnPasswordChanged;
    }

    void OnPasswordChanged(object uielement, RoutedEventArgs e)
    {
        var passwordbox = uielement.As<PasswordBox>();
        PasscodeHasText.SetValue(passwordbox, passwordbox.Password.Length > 0);
    }
}

internal sealed class PasscodeHasText : Behavior<PasscodeHasText, bool>
{
    protected override void OnValueChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e) { }
}

