namespace Parseh.UI.Resources;

using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Navigation;

internal sealed class PasscodeHasPlaceholder : AttachedProperty<PasscodeHasPlaceholder, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
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

internal sealed class PasscodeHasText : AttachedProperty<PasscodeHasText, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e) { }
}

internal sealed class NoFrameHistory : AttachedProperty<NoFrameHistory, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (!e.NewValue.As<bool>()) return;

        var frame = uielement.As<Frame>();
        frame.JournalOwnership = JournalOwnership.OwnsJournal;
        frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
        frame.Navigated += (sender, _) => sender.As<Frame>().RemoveBackEntry();
    }
}

#region Chat Menu

internal sealed class ShowSearchbar : SimpleAnimatedProperty<ShowSearchbar>
{
    bool firstload = true;

    protected override async void DoAnimate(FrameworkElement element, bool value)
    {
        if (!firstload)
        {
            const double Duration = 0.4;
            var offset = element.Height;
            if (value) await element.TranslateAsync(new(0), new(0, offset, 0, -offset), Duration);
            else await element.TranslateAsync(new(0, offset, 0, -offset), new(0), Duration);
        }
        else firstload = false;
    }
}

internal sealed class HideSearchbar : SimpleAnimatedProperty<HideSearchbar>
{
    bool firstload = true;

    protected override async void DoAnimate(FrameworkElement element, bool value)
    {
        var offset = element.Height;
        if (!firstload)
        {
            const double Duration = 0.4;
            if (value) await element.TranslateAsync(new(0, -offset, 0, offset), new(0), Duration);
            else await element.TranslateAsync(new(0), new(0, -offset, 0, offset), Duration);
        }
        else
        {
            await element.TranslateAsync(new(0, -offset, 0, offset), new(0), 0);
            firstload = false;
        }
    }
}



#endregion


internal sealed class FocusOnLoad : AttachedProperty<FocusOnLoad, bool>
{
    // TODO: برای سرچ بار کار نمیکند
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull()) return;

        var textblock = uielement.As<TextBox>();

        textblock.Loaded += (sender, ee) => App.Dispatch(() => sender.As<TextBox>().Focus());
    }
}