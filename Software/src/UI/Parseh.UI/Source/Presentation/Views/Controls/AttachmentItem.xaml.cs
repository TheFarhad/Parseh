namespace Parseh.UI.Views;

using System.Windows.Media;
using System.Windows.Media.Imaging;

public partial class AttachmentItem : Component
{
    public static readonly DependencyProperty AttachmentTitleProperty =
        DependencyProperty.Register(nameof(AttachmentTitle), typeof(string), typeof(AttachmentItem),
            new PropertyMetadata(default(string)));

    public static readonly DependencyProperty AttachmentImageSourceProperty =
        DependencyProperty.Register(nameof(AttachmentImageSource), typeof(BitmapSource), typeof(AttachmentItem),
            new PropertyMetadata(default(ImageSource)));

    public string AttachmentTitle
    {
        get => GetValue(AttachmentTitleProperty).As<string>();
        set => SetValue(AttachmentTitleProperty, value);
    }

    public BitmapSource AttachmentImageSource
    {
        get => GetValue(AttachmentImageSourceProperty).As<BitmapSource>();
        set => SetValue(AttachmentImageSourceProperty, value);
    }

    public AttachmentItem() => Init();

    void Init()
    {
        InitializeComponent();
    }


}
