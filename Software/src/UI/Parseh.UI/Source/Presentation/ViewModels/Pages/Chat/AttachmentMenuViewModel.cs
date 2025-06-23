namespace Parseh.UI.ViewModels;

public sealed class AttachmentMenuViewModel : PopupMenuViewModel
{
    public AttachmentMenuViewModel() => Init();

    void Init()
    {
        CornerRadius = new(0, 10, 0, 0);
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment = VerticalAlignment.Bottom;
        Margin = new Thickness(3, 0, 0, 52);
        Items = [
             new PopupMenuItemViewModel
            {
                Title = "Gallery",
                Icon = "",
                Type = PopupMenuItemType.Header
            },
            new PopupMenuItemViewModel
            {
                Title = "Photo or video",
                Icon = "Media",
                Type = PopupMenuItemType.TextAndIcon
            },
              new PopupMenuItemViewModel
            {
                Title = "Document",
                Icon = "Document",
                Type = PopupMenuItemType.TextAndIcon
            },
             new PopupMenuItemViewModel
            {
                Title = "",
                Icon = "",
                Type = PopupMenuItemType.Divider
            },
             new PopupMenuItemViewModel
            {
                Title = "Map",
                Icon = "",
                Type = PopupMenuItemType.Header
            },
             new PopupMenuItemViewModel
            {
                Title = "Location",
                Icon = "Location",
                Type = PopupMenuItemType.TextAndIcon
            }
            ];
    }
}





