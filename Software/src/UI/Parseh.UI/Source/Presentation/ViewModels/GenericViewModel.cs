namespace Parseh.UI.ViewModels;

internal sealed class GenericViewModel : VM
{
    public ContentPage Page { get => Get(); private set => Set(value); }
    public ViewModel ViewModel { get => Get(); private set => Set(value); }

    public GenericViewModel() => Init();

    public void ToPage(PageMode page)
    {
        Page = Pager(page);
    }

    #region Private Functionality

    void Init()
    {
        Page = Pager(PageMode.Signin);
    }

    ContentPage Pager(PageMode page)
        => page switch
        {
            PageMode.Signin => new Signin(),
            PageMode.Signup => new Signup(),
            PageMode.Chat => new Chat(),
            _ => throw new NotImplementedException(""),
        };

    #endregion
}

internal sealed class Generic
{
    public static readonly Generic Self = new();

    Generic()
    {
        var ioc = NetCoreIoC.Self;
        ViewModel = ioc.GetRequired<GenericViewModel>();
    }

    public GenericViewModel ViewModel { get; }
}