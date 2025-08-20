namespace Parseh.UI.ViewModels;

public sealed class CortexViewModel : ViewModel
{
    private readonly IServiceProvider _serviceProvider;

    public PageMode Mode { get => Get(); private set => Set(value); }
    public ContentPage Page { get => Get(); private set => Set(value); }
    public SettingMenuViewModel SettingMenuModel { get => Get(); set => Set(value); }

    public CortexViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Init();
    }

    public void ToPage(PageMode mode)
    {
        if (Mode != mode) Pager(mode);
    }

    #region Private Functionality

    private void Init()
    {
        Mode = PageMode.Signin;
        Pager(Mode);
    }

    private void Pager(PageMode mode)
    {
        try
        {
            Page = mode switch
            {
                PageMode.Signin => _serviceProvider.GetRequiredService<Signin>(),
                PageMode.Signup => _serviceProvider.GetRequiredService<Signup>(),
                PageMode.Chat => _serviceProvider.GetRequiredService<Chat>(),
                _ => throw new NotImplementedException("A page for this mode has not been implemented."),
            };
        }
        catch (Exception)
        {
            Page = null!;
        }
        Mode = mode;
    }

    #endregion
}

//internal sealed class Cortex
//{
//    readonly Ioc _ioc = default!;
//    public static readonly Cortex Default = new();

//    private Cortex()
//    {
//        _ioc = Ioc.Default;
//    }

//    public CortexViewModel Model => _ioc.CortexViewModel;
//}