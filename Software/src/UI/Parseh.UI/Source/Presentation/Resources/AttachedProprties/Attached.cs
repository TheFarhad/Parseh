namespace Parseh.UI.Resources;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
using Views;
using System.Xml.Linq;

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
        if (e.NewValue.As<bool>())
        {
            uielement?.Is<Frame>(frame =>
            {
                frame.JournalOwnership = JournalOwnership.OwnsJournal;
                frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                frame.Navigated += (sender, _) => frame.RemoveBackEntry();
            });
        }
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

#region Focus

internal sealed class Focus : AttachedProperty<Focus, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<Control>(control =>
        {
            if (e.NewValue.As<bool>())
                control.Focus();
        });
    }
}

internal sealed class FocusOnLoad : AttachedProperty<FocusOnLoad, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement.Is<Control>(control =>
        {
            if (e.NewValue.As<bool>())
                control.Loaded += (sender, ee) => control.Focus();
        });
    }
}

internal sealed class FocusAndSelectText : AttachedProperty<FocusAndSelectText, bool>
{
    public override void OnCoerceValue(DependencyObject uielement, object value)
    {
        if (value.As<bool>())
        {
            uielement.Is<TextBoxBase>(textbox =>
            {
                textbox.Focus();
                textbox.SelectAll();
            });
            uielement.Is<PasswordBox>(passcode =>
            {
                passcode.Focus();
                passcode.SelectAll();
            });
        }
    }
}

#endregion

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

#endregion

internal sealed class ChangeVisibility : AttachedProperty<ChangeVisibility, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<FrameworkElement>(element =>
        {
            if (e.NewValue.As<bool>()) element.Visibility = Visibility.Visible;
            else element.Visibility = Visibility.Collapsed;
        });
    }
}

internal sealed class FadeAnimate : AttachedProperty<FadeAnimate, bool>
{
    const double Duration = 0.3;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<FrameworkElement>(async element =>
        {
            if (e.NewValue.As<bool>()) await element.FadeToAsync(0, 1, Duration);
            else await element.FadeToAsync(1, 0, Duration);
        });
    }
}

internal sealed class ZIndex : AttachedProperty<ZIndex, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<FrameworkElement>(element =>
        {
            if (e.NewValue.As<bool>()) Panel.SetZIndex(element, 1);
            else Panel.SetZIndex(element, 0);
        });
    }
}

internal sealed class ZIndexFadeVisibilityAnimate : AttachedProperty<ZIndexFadeVisibilityAnimate, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<FrameworkElement>(element =>
        {
            var val = e.NewValue.As<bool>();
            if (val)
            {
                ChangeVisibility.SetValue(element, val);
                ZIndex.SetValue(element, val);
                FadeAnimate.SetValue(element, val);
            }
            else
            {
                // TODO: برای المانهای پسکود، در حالت فیداوت کار نمیکند
                // انیمیشن در این حالت کار نمی کند

                FadeAnimate.SetValue(element, val);
                ZIndex.SetValue(element, val);
                ChangeVisibility.SetValue(element, val);
            }
        });
    }
}

#region Custome Controls

internal sealed class EntryEditingVisibility : AttachedProperty<EntryEditingVisibility, bool>
{
    double _duration;
    bool _firstload = true;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        if (_firstload)
        {
            _firstload = false;
            _duration = 0;
        }
        else _duration = 0.2;
        uielement.Is<FrameworkElement>(/*async*/ element =>
        {
            if (e.NewValue.As<bool>())
            {
                element.Visibility = Visibility.Visible;
                App.DispatchAsync(() => element.FadeToAsync(0, 1, _duration));
                //await element.FadeToAsync(0, 1, _duration);
            }
            else
            {
                //await element.FadeToAsync(1, 0, _duration);
                App.DispatchAsync(() => element.FadeToAsync(1, 0, _duration));
                element.Visibility = Visibility.Collapsed;
            }
        });
    }
}

internal sealed class EntryEditMode : AttachedProperty<EntryEditMode, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<TextBox>(element =>
        {
            if (e.NewValue.As<bool>())
            {
                element.Focus();
                element.SelectAll();
            }
        });
    }
}

internal sealed class PasscodeEditMode : AttachedProperty<PasscodeEditMode, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<PasswordBox>(element =>
        {
            if (e.NewValue.As<bool>())
            {
                element.Focus();
                element.SelectAll();
            }
        });
    }
}

#endregion


internal sealed class PanelMatchChildWidth : AttachedProperty<PanelMatchChildWidth, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<Panel>(panel =>
        {
            RoutedEventHandler onLoaded = null!;
            onLoaded = (s, ee) =>
            {
                panel.Loaded -= onLoaded;

                SetEqualWidth(panel);

                foreach (var item in panel.Children)
                {
                    //item.Is<Entry>(entry => entry.Label.SizeChanged += (ss, eee) => SetEqualWidth(panel));
                    //item.Is<Passcode>(passcode => entry.Label.SizeChanged += (ss, eee) => SetEqualWidth(panel));
                }
            };
            panel.Loaded += onLoaded;
        });
    }

    void SetEqualWidth(Panel panel)
    {
        var maxsize = 0.0;
        foreach (var item in panel.Children)
        {
            //var labelColumnWidth = entry.Label.RenderSize.Width + entry.Label.Margin.Left + entry.Label.Margin.Right;
            item.Is<Entry>(entry => maxsize = Math.Max(maxsize, entry.LabelColumn.ActualWidth));
            item.Is<Passcode>(passcode => maxsize = Math.Max(maxsize, passcode.LabelColumn.ActualWidth));
        }
        foreach (var item in panel.Children)
        {
            item.Is<Entry>(entry => entry.LabelColumn.Width = new GridLength(maxsize));
            item.Is<Passcode>(passcode => passcode.LabelColumn.Width = new GridLength(maxsize));
        }

    }
}


internal sealed class DataContextChanged : AttachedProperty<DataContextChanged, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<FrameworkElement>(element => element.DataContextChanged += OnDataContextChanged);
    }

    async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => await sender.As<FrameworkElement>().FadeToAsync(duration: 0.3);
}



