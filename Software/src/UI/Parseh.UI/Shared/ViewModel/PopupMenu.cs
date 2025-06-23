namespace Parseh.UI.Views;

public sealed class PopupMenuItemViewModel : VM
{
    public string Title { get => Get(); set => Set(value); }
    public PopupMenuItemType Type { get => Get(); set => Set(value); }
    public string Icon { get => Get(); set => Set(value); }

    public PopupMenuItemViewModel() => Init();

    void Init()
    {

        Type = PopupMenuItemType.Header;
    }
}

public class PopupMenuViewModel : VM
{
    public CornerRadius CornerRadius { get => Get(); protected set => Set(value); }
    public Thickness Padding { get => Get(); protected set => Set(value); }
    public HorizontalAlignment HorizontalAlignment { get => Get(); protected set => Set(value); }
    public VerticalAlignment VerticalAlignment { get => Get(); protected set => Set(value); }
    public Thickness Margin { get => Get(); protected set => Set(value); }
    public ObservableSet<PopupMenuItemViewModel> Items { get => Get(); protected set => Set(value); }

    public PopupMenuViewModel() => Init();

    void Init()
    {
        CornerRadius = new(Constant.CornerRadius, 0, Constant.CornerRadius, 0);
        Padding = new(5);
        Items = [];
    }
}

public enum PopupMenuItemType
{
    Header = 0,
    Divider = 1,
    TextAndIcon = 2
}
