namespace Parseh.UI.ViewModels;

public sealed class CortexViewModel : ViewModel
{
    public CortexViewModel()
        => Init();

    public PageMode Mode { get => Get(); private set => Set(value); }
    public ContentPage Page { get => Get(); private set => Set(value); }
    public ProfileViewModel SettingMenuModel { get => Get(); set => Set(value); }

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
                PageMode.Signin => App.RequiredService<Signin>(),
                PageMode.Signup => App.RequiredService<Signup>(),
                PageMode.Chat => App.RequiredService<Chat>(),
                _ => throw new NotImplementedException("A page for this mode has not been implemented."),
            };
            Mode = mode;
        }
        catch (Exception)
        {
            Page = null!;
            Mode = PageMode.Unspecified;
        }
    }

    #endregion
}