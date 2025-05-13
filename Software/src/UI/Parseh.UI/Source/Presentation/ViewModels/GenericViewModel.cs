namespace Parseh.UI.ViewModels;

internal sealed class GenericViewModel : VM
{
    public PageMode Mode { get => Get(); private set => Set(value); }
    public ContentPage Page { get => Get(); private set => Set(value); }

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