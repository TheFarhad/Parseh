namespace Parseh.UI.Resources;

using System.Windows;
using System.Windows.Navigation;
using System.Xml.Linq;
using Views;

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

#region Mouse-Over Commands [very useful]

internal sealed class MouseOverCommandParamrter : AttachedProperty<MouseOverCommandParamrter, object>
{

}

internal sealed class MouseEnterCommand : AttachedProperty<MouseEnterCommand, IRelayCommand>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
        => uielement.Is<UIElement>(element => element.MouseEnter += OnMouseEnter);

    void OnMouseEnter(object sender, MouseEventArgs e)
    {
        var element = sender.As<UIElement>();
        var parameter = MouseOverCommandParamrter.GetValue(element);
        var command = GetValue(element).As<IRelayCommand>();
        if (command?.CanExecute(parameter) is true) command.Execute(parameter);
    }
}

internal sealed class MouseLeaveCommand : AttachedProperty<MouseLeaveCommand, IRelayCommand>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
        => uielement.Is<UIElement>(element => element.MouseLeave += OnMouseLeave);

    void OnMouseLeave(object sender, MouseEventArgs e)
    {
        var element = sender.As<UIElement>();
        var parameter = MouseOverCommandParamrter.GetValue(element);
        var command = GetValue(element).As<IRelayCommand>();
        if (command?.CanExecute(parameter) is true) command.Execute(parameter);
    }
}

#endregion

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
    const double Duration = 0.8;
    const double Angle = 45;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull())
            return;

        if (e.NewValue.As<bool>()) App.DispatchAsync(() => uielement.As<Button>().RotateTo(0, Angle, Duration));
        else App.DispatchAsync(() => uielement.As<Button>().RotateTo(Angle, 0, Duration));
    }
}

#endregion


internal sealed class FocusOnLoad : AttachedProperty<FocusOnLoad, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
        => uielement?.As<TextBox>().Focus();
}

#region Setting Menu

internal sealed class SettingMenuAnimate : AttachedProperty<SettingMenuAnimate, bool>
{
    const double Duration = 0.3;
    const double Offset = 70;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull())
            return;

        uielement.Is<Border>(settingMenuBorder =>
        {
            var value = e.NewValue.As<bool>();

            if (value) App.DispatchAsync(() => settingMenuBorder.TranlateFadeAsync(new(Offset, 0, -Offset, 0), new(0), 0, 1, Duration));
            else App.DispatchAsync(() => settingMenuBorder.FadeToAsync(1, 0, Duration));
        });
    }
}

internal sealed class SettingMenuAnimatedVisibility : AttachedProperty<SettingMenuAnimatedVisibility, bool>
{
    const double Duration = 0.3;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull())
            return;

        uielement.Is<Setting>(async setting =>
        {
            var value = e.NewValue.As<bool>();
            if (value)
            {
                setting.Visibility = Visibility.Visible;
                Panel.SetZIndex(setting, 1);
                await setting.FadeToAsync(0, 1, Duration);
            }
            else
            {
                await setting.FadeToAsync(1, 0, Duration);
                Panel.SetZIndex(setting, 0);
                setting.Visibility = Visibility.Collapsed;
            }
        });
    }
}

#endregion

internal sealed class ShowAttachmentMenu : AttachedProperty<ShowAttachmentMenu, bool>
{
    const double Duration = 0.4;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull())
            return;

        uielement.Is<Border>(async attachment =>
        {
            if (e.NewValue.As<bool>()) await attachment.FadeToAsync(0, 1, Duration);
            else await attachment.FadeToAsync(1, 0, Duration);
        });
    }
}

internal sealed class AttachmentMenuVisibility : AttachedProperty<AttachmentMenuVisibility, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (uielement.IsNull())
            return;

        uielement.Is<Attachment>(attachment =>
        {
            if (e.NewValue.As<bool>()) Panel.SetZIndex(attachment, 1);
            else Panel.SetZIndex(attachment, 0);
        });
    }
}





