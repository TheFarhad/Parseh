namespace Parseh.UI.Resources;

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
        if (firstload) firstload = false;
        else
        {
            const double Duration = 0.2;
            var offset = element.Height;
            if (value) await element.TranslateAsync(new(0), new(0, offset, 0, -offset), Duration);
            else await element.TranslateAsync(new(0, offset, 0, -offset), new(0), Duration);
        }
    }
}

internal sealed class HideSearchbar : SimpleAnimatedProperty<HideSearchbar>
{
    bool firstload = true;

    protected override async void DoAnimate(FrameworkElement element, bool value)
    {
        var offset = element.Height;
        if (firstload)
        {
            await element.TranslateAsync(new(0, -offset, 0, offset), new(0), 0);
            firstload = false;
        }
        else
        {
            const double Duration = 0.2;
            if (value) await element.TranslateAsync(new(0, -offset, 0, offset), new(0), Duration);
            else await element.TranslateAsync(new(0), new(0, -offset, 0, offset), Duration);
        }
    }
}

internal sealed class CloseSearchbarButtonRotateAnimate : AttachedProperty<CloseSearchbarButtonRotateAnimate, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull())
            return;

        if (e.NewValue.As<bool>()) App.Dispatch(() => uielement.As<Button>().RotateTo(0, 45, 0.8));
        else App.Dispatch(() => uielement.As<Button>().RotateTo(0, 45, 0.8));
    }
}

#endregion


internal sealed class FocusOnLoad : AttachedProperty<FocusOnLoad, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.As<TextBox>().Focus();
    }
}

#region Setting Menu

internal sealed class SettingMenuAnimate : AttachedProperty<SettingMenuAnimate, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull())
            return;

        var settingMenu = uielement.As<Border>();
        var value = e.NewValue.As<bool>();
        var offset = 70;
        var duration = 0.3;

        if (value) App.Dispatch(() => settingMenu.TranlateFadeAsync(new(offset, 0, -offset, 0), new(0), 0, 1, duration));
        //else App.Dispatch(() => settingMenu.TranlateFadeAsync(new(0), new(-offset, 0, offset, 0), 1, 0, duration));
        else App.Dispatch(() => settingMenu.FadeToAsync(1, 0, duration));
    }
}

internal sealed class SettingMenuZIndexAnimate : AttachedProperty<SettingMenuZIndexAnimate, bool>
{
    public override async void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull())
            return;

        var settingContainer = uielement.As<Setting>();
        var value = e.NewValue.As<bool>();
        var duration = 0.3;

        if (value)
        {
            settingContainer.Visibility = Visibility.Visible;
            Panel.SetZIndex(settingContainer, 1);
            await settingContainer.FadeToAsync(0, 1, duration);
        }
        else
        {
            await settingContainer.FadeToAsync(1, 0, duration);
            Panel.SetZIndex(settingContainer, 0);
            settingContainer.Visibility = Visibility.Collapsed;
        }
    }
}

#endregion



