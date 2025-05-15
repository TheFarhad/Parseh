namespace Parseh.UI.Views;

using System.Windows.Media;

public partial class AttachmentItem : Component
{
    public static readonly DependencyProperty AttachmentTitleProperty =
        DependencyProperty.Register(nameof(AttachmentTitle), typeof(string), typeof(AttachmentItem),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty AttachmentImageSourceProperty =
        DependencyProperty.Register(nameof(AttachmentImageSource), typeof(ImageSource), typeof(AttachmentItem),
            new PropertyMetadata(default(ImageSource)));

    public string AttachmentTitle
    {
        get => GetValue(AttachmentTitleProperty).As<string>();
        set => SetValue(AttachmentTitleProperty, value);
    }

    public ImageSource AttachmentImageSource
    {
        get => GetValue(AttachmentImageSourceProperty).As<ImageSource>();
        set => SetValue(AttachmentImageSourceProperty, value);
    }

    public AttachmentItem() => Init();

    void Init()
    {
        InitializeComponent();
    }
}
