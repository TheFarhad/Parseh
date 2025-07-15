namespace Parseh.UI.Resources;

using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;

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
        uielement?.Is<Frame>(frame =>
        {
            if (e.NewValue.As<bool>())
            {
                frame.JournalOwnership = JournalOwnership.OwnsJournal;
                frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
                frame.Navigated += (sender, _) => frame.RemoveBackEntry();
            }
        });
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
    const double Duration = 0.3;
    bool firstload = true;

    protected override async void DoAnimate(FrameworkElement element, bool value)
    {
        if (firstload)
        {
            firstload = false;
            return;
        }

        var offset = element.RenderSize.Height + (element.Parent.As<Grid>().Margin.Top * 2);

        if (value) await element.TranslateAsync(new(0, -offset, 0, offset), new(0), Duration);
        else await element.TranslateAsync(new(0), new(0, -offset, 0, offset), Duration);
    }
}

internal sealed class ShowMenu : SimpleAnimatedProperty<ShowMenu>
{
    const double Duration = 0.3;
    bool firstload = true;

    protected override async void DoAnimate(FrameworkElement element, bool value)
    {
        if (firstload)
        {
            firstload = false;
            return;
        }

        var offset = element.RenderSize.Height + (element.Parent.As<Grid>().Margin.Top * 2);

        if (value) await element.TranslateAsync(new(0), new(0, offset, 0, -offset), Duration);
        else await element.TranslateAsync(new(0, offset, 0, -offset), new(0), Duration);
    }
}


internal sealed class CloseSearchbarButtonRotateAnimate : AttachedProperty<CloseSearchbarButtonRotateAnimate, bool>
{
    const double Duration = 0.3;
    const double Angle = 45;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<Button>(element =>
        {
            if (e.NewValue.As<bool>()) App.DispatchAsync(() => element.RotateTo(0, Angle, Duration));
            else App.DispatchAsync(() => element.RotateTo(Angle, 0, Duration));
        });
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
        uielement?.Is<Border>(async element =>
        {
            if (e.NewValue.As<bool>() is true)
                await element.TranlateFadeAsync(new(Offset, 0, -Offset, 0), new(0), 0, 1, Duration);
            else
                await element.FadeToAsync(1, 0, Duration);
        });
    }
}

internal sealed class SettingMenuOverlayFadeAnimate : AttachedProperty<SettingMenuOverlayFadeAnimate, bool>
{
    const double Duration = 0.3;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<FrameworkElement>(async element =>
        {
            var val = e.NewValue.As<bool>();
            if (val)
                await element.FadeToAsync(0, 0.5, Duration);
            else
                await element.FadeToAsync(0.5, 0, Duration);
        });
    }
}

internal sealed class SettingMenuZIndexAnimate : AttachedProperty<SettingMenuZIndexAnimate, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<FrameworkElement>(async element =>
        {
            if (e.NewValue.As<bool>() is true)
                Panel.SetZIndex(element, 1);
            else
            {
                await Task.Delay(300);
                Panel.SetZIndex(element, 0);
            }
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

#region Entry and Passcode

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
        List<double> sizes = [];
        foreach (var item in panel.Children)
        {
            //var labelColumnWidth = entry.Label.RenderSize.Width + entry.Label.Margin.Left + entry.Label.Margin.Right;
            item.Is<Entry>(entry => sizes.Add(entry.LabelColumn.ActualWidth));
            item.Is<Passcode>(passcode => sizes.Add(passcode.LabelColumn.ActualWidth));
        }
        var maxsize = sizes.Max();
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
        uielement?.Is<ChatMessageContainerCard>(element => element.DataContextChanged += async (ee, ss) =>
        {
            await element.As<ChatMessageContainerCard>().FadeToAsync(duration: 0.3);
        });
    }

    async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => await sender.As<ChatMessageContainerCard>().FadeToAsync(duration: 0.3);
}
#region ScrollViewer

internal sealed class ScrollToBottomOnDataContextChanged : AttachedProperty<ScrollToBottomOnDataContextChanged, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<ScrollViewer>(scrollViewer =>
        {
            scrollViewer.DataContextChanged -= OnDataContextChanged;
            scrollViewer.DataContextChanged += OnDataContextChanged;
        });
    }

    void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        => sender.As<ScrollViewer>().ScrollToBottom();
}

internal sealed class ScrollToBottom : AttachedProperty<ScrollToBottom, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
        => uielement?.Is<ScrollViewer>(element =>
        {
            if (e.NewValue.As<bool>() is true)
            {
                var currentPosition = element.VerticalOffset;
                var EndPosition = element.ScrollableHeight;

                var da = new DoubleAnimation();
                da.From = currentPosition;
                da.To = EndPosition;
                da.Duration = TimeSpan.FromSeconds(0.2);
                da.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn };

                var storyboard = new Storyboard();
                storyboard.Children.Add(da);

                Storyboard.SetTarget(da, element);
                Storyboard.SetTargetProperty(da, new PropertyPath(ScrollViewerHelper.CurrentVerticalScrollPositionProperty));

                storyboard.Begin(element);
            }
        });
}

public static class ScrollViewerHelper
{
    public static readonly DependencyProperty CurrentVerticalScrollPositionProperty =
        DependencyProperty.RegisterAttached(
            "CurrentVerticalScrollPosition",
            typeof(double),
            typeof(ScrollViewerHelper),
            new PropertyMetadata(0.0, OnCurrentVerticalScrollPositionChanged));

    public static double GetCurrentVerticalScrollPosition(DependencyObject uielement)
        => uielement.GetValue(CurrentVerticalScrollPositionProperty).As<double>();

    public static void SetCurrentVerticalScrollPosition(DependencyObject uielement, double value)
        => uielement.SetValue(CurrentVerticalScrollPositionProperty, value);

    static void OnCurrentVerticalScrollPositionChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
        => uielement?.Is<ScrollViewer>(elememt => elememt.ScrollToVerticalOffset(e.NewValue.As<double>()));
}

internal sealed class ScrollChanged : AttachedProperty<ScrollChanged, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<ScrollViewer>(element =>
        {
            element.ScrollChanged += (ss, ee) =>
            {
                element.DataContext?.Is<ContactChatViewModel>(viewmodel =>
                {
                    if (element.ScrollableHeight - element.VerticalOffset > 80)
                        viewmodel.ShowChevronDownButton = true;

                    else
                    {
                        viewmodel.ShowChevronDownButton = false;
                        viewmodel.ScrollToLastMessage = false;
                    }
                });

            };
        });
    }
}

#endregion

internal sealed class MicAndSendButtonVisibility : AttachedProperty<MicAndSendButtonVisibility, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<FrameworkElement>(async element =>
        {
            var val = e.NewValue.As<bool>();
            if (val)
            {
                ZIndex.SetValue(element, val);
                await element.FadeToAsync(0, 0.5, 0.2);
            }
            else
            {
                await element.FadeToAsync(0.5, 0, 0.2);
                ZIndex.SetValue(element, val);
            }
        });
    }
}

internal sealed class NewMessageAnimation : AttachedProperty<NewMessageAnimation, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<ChatMessageCard>(element =>
        {
            if (e.NewValue.As<bool>())
            {
                var _showPosition = element.Margin;
                var _hidePosition = new Thickness(_showPosition.Left, _showPosition.Top, _showPosition.Right, -40);

                element.Loaded += async (ss, ee) => await element.TranslateAsync(_showPosition, _showPosition, 0.15);
            }
        });
    }
}

internal sealed class ChveronDownButtonVisisbility : AttachedProperty<ChveronDownButtonVisisbility, bool>
{
    const double Duration = 0.3;
    bool firstload = true;
    Thickness _showPosition;
    Thickness _hidePosition;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<Button>(async element =>
        {
            if (firstload)
            {
                _showPosition = element.Margin;
                _hidePosition = new Thickness(_showPosition.Left, _showPosition.Top, _showPosition.Right, -60);
                firstload = false;
            }

            var val = e.NewValue.As<bool>();
            if (val)
            {
                ZIndex.SetValue(element, val);
                await element.TranslateAsync(_hidePosition, _showPosition, Duration);
            }
            else
            {
                await element.TranslateAsync(_showPosition, _hidePosition, Duration);
                ZIndex.SetValue(element, val);
            }
        });
    }
}

#region Animate Image OnLoad

internal sealed class ClipImageInBorder : AttachedProperty<ClipImageInBorder, bool>
{
    RoutedEventHandler _onBorderLoaded;
    SizeChangedEventHandler _onBorderSizeChanged;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<Image>(image =>
        {
            image.Parent?.Is<Border>(border =>
            {
                _onBorderLoaded = (ss, ee) => OnBorderLoaded(ss, ee, image);
                _onBorderSizeChanged = (ss, ee) => OnBorderLoaded(ss, ee, image);

                var val = e.NewValue.As<bool>();
                if (val)
                {
                    border.Loaded += _onBorderLoaded;
                    border.SizeChanged += _onBorderSizeChanged;
                }
                else
                {
                    border.Loaded += _onBorderLoaded;
                    border.SizeChanged += _onBorderSizeChanged;
                }
            });
        });
    }

    void OnBorderLoaded(object sender, RoutedEventArgs e, FrameworkElement element)
    {
        var border = sender.As<Border>();

        if (border is { ActualWidth: 0, ActualHeight: 0 })
            return;

        var rg = new RectangleGeometry();
        rg.RadiusX = rg.RadiusY = 4;
        rg.Rect = new Rect(border.RenderSize);
        element.Clip = rg;
    }
}

#endregion

#region Attachment Menu

internal sealed class AttachmentButtonMousOver : AttachedProperty<AttachmentButtonMousOver, bool>
{
    MouseEventHandler _onMouseEnter;
    MouseEventHandler _onMouseLeave;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<Border>(element =>
        {
            var val = e.NewValue.As<bool>();

            _onMouseEnter = (ss, ee) => OnMouseEnter(element, ee, val);
            _onMouseLeave = async (ss, ee) => await OnMouseLeave(element, ee, val);

            element.MouseEnter -= _onMouseEnter;
            element.MouseLeave -= _onMouseLeave;

            element.MouseEnter += _onMouseEnter;
            element.MouseLeave += _onMouseLeave;
        });
    }

    void OnMouseEnter(Border button, MouseEventArgs e, bool value)
        => button.DataContext.As<ChatViewModel>().IsOpenAttachmentMenu = true;

    async Task OnMouseLeave(Border button, MouseEventArgs e, bool value)
    {
        await Task.Delay(200);
        button.DataContext.As<ChatViewModel>().IsOpenAttachmentMenu = false;
    }
}

internal sealed class AttachmentMenuMouseOver : AttachedProperty<AttachmentMenuMouseOver, bool>
{
    const double Duration = 0.1;

    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<AttachmentMenu>(async element =>
        {
            element.MouseLeave += async (ee, ss) =>
            {
                await element.FadeToAsync(1, 0, Duration);
                Panel.SetZIndex(element, 0);
            };

            if (element.IsMouseOver)
            {
                Panel.SetZIndex(element, 1);
                element.Opacity = 1;
            }
            else
            {
                var val = e.NewValue.As<bool>();
                if (val)
                {
                    Panel.SetZIndex(element, 1);
                    await element.FadeToAsync(0, 1, Duration);
                }
                else
                {
                    await element.FadeToAsync(1, 0, Duration);
                    Panel.SetZIndex(element, 0);
                }
            }
        });
    }
}

#endregion

internal sealed class ImageMassageFadeInOnLoad : AttachedProperty<ImageMassageFadeInOnLoad, bool>
{
    public override void OnPropertyChanged(DependencyObject uielement, DependencyPropertyChangedEventArgs e)
    {
        uielement?.Is<Image>(element =>
        {
            if (e.NewValue.As<bool>())
                // در اینجا یعنی وقتی که مقدار پراپرتی سورس از ایمیج تغییر میکند
                element.TargetUpdated += OnTargetUpdated;
            else
                element.TargetUpdated -= OnTargetUpdated;
        });
    }

    async void OnTargetUpdated(object? sender, DataTransferEventArgs e)
        => await sender!.As<Image>().FadeToAsync();
}


