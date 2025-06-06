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
    public double CornerRadius { get => Get(); private set => Set(value); }
    public Thickness Padding { get => Get(); set => Set(value); }
    public HorizontalAlignment HorizontalAlignment { get => Get(); set => Set(value); }
    public VerticalAlignment VerticalAlignment { get => Get(); set => Set(value); }
    public Thickness Margin { get => Get(); set => Set(value); }
    public ObservableSet<PopupMenuItemViewModel> Items { get => Get(); protected set => Set(value); }

    public PopupMenuViewModel() => Init();

    void Init()
    {
        CornerRadius = Constant.CornerRadius;
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
