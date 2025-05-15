namespace Parseh.UI.ViewModels;

internal sealed class GenericViewModel : VM
{
    // TODO: گرید مربوط به منو ستینگ رو کلا به یه کامپوننت جدا تبدیلش کن
    // اینطوری اینپوت بایندینگ آن هم از لی اوت ویومدل به ویومدل خود ستینگ منتقل می شود
    // یک کلاس استاتیک درست کن که یکسری مقادیر پیش فرض درون آن باشند، مثل کرنر رادیوس

    public PageMode Mode { get => Get(); private set => Set(value); }
    public ContentPage Page { get => Get(); private set => Set(value); }
    public bool IsOpenSettingMenu { get => Get(); set => Set(value); }
    public bool IsOpenAttachmentMenu { get => Get(); set => Set(value); }

    public GenericViewModel() => Init();

    public void ToPage(PageMode mode)
    {
        if (Mode != mode) Pager(mode);
    }

    #region Private Functionality

    void Init()
    {
        Mode = PageMode.Chat;
        Pager(Mode);
        IsOpenSettingMenu = false;
        IsOpenAttachmentMenu = false;
    }

    void Pager(PageMode mode)
    {
        Page = mode switch
        {
            PageMode.Signin => new Signin(),
            PageMode.Signup => new Signup(),
            PageMode.Chat => new Chat(),
            _ => throw new NotImplementedException("A page for this mode has not been implemented."),
        };
        Mode = mode;
    }

    #endregion
}

internal sealed class Generic
{
    private static readonly Generic _default = default!;
    public static readonly Generic Default = new();

    Generic()
    {
        var ioc = NetIoC.Default;
        Model = ioc.GetRequired<GenericViewModel>();
    }

    public GenericViewModel Model { get; }
}