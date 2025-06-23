namespace Parseh.UI.ViewModels;

internal sealed class CortexViewModel : VM
{
    public PageMode Mode { get => Get(); private set => Set(value); }
    public ContentPage Page { get => Get(); private set => Set(value); }
    public SettingMenuViewModel SettingMenuModel { get => Get(); set => Set(value); }

    public CortexViewModel() => Init();

    public void ToPage(PageMode mode)
    {
        if (Mode != mode) Pager(mode);
    }

    #region Private Functionality

    void Init()
    {
        Mode = PageMode.Chat;
        Pager(Mode);
        SettingMenuModel = new();
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

internal sealed class Cortex
{
    readonly Ioc _ioc = default!;
    public static readonly Cortex Default = new();

    Cortex()
    {
        _ioc = Ioc.Default;
    }

    public CortexViewModel Model => _ioc.CortexViewModel;
}

//internal sealed class Cortex
//{
//    public CortexViewModel Model => NetIoC.Default.GetRequired<CortexViewModel>();
//    public INotifierService Notifier => NetIoC.Default.GetRequired<INotifierService>();
//}