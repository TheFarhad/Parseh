namespace Parseh.UI.ViewModels;

public sealed class ChatMessageViewModel : VM
{
    #region Properties

    public string Message
    {
        get => Get();
        set
        {
            Set(value);
            Notify(nameof(HasMessage));
        }
    }
    public string Sender { get => Get(); set => Set(value); }
    public DateTimeOffset SendAt { get => Get(); set => Set(value); }
    public DateTimeOffset ReadAt { get => Get(); set => Set(value); }
    public bool SendByMe { get => Get(); set => Set(value); }
    public bool IsNewMessage { get => Get(); set => Set(value); }
    public ContactMessageImageAttachment Image
    {
        get => Get();
        set
        {
            Set(value);
            Notify(nameof(HasImage));
        }
    }
    public bool HasMessage => Message.IsNotEmpty();
    public bool HasImage => Image.IsNotNull();

    #endregion

    public ChatMessageViewModel() => Init();

    #region Private Functionality

    void Init()
    {
        InitProperties();
        InitCommands();
    }

    void InitProperties()
    {
        IsNewMessage = false;
    }

    void InitCommands()
    {

    }

    #endregion

    #region Command Methods

    #endregion
}

public sealed class ContactMessageImageAttachment : VM
{
    public string Title { get => Get(); set => Set(value); }
    public string FileName { get => Get(); set => Set(value); }
    public long Size { get => Get(); set => Set(value); }
    public string ThumbnailUrl
    {
        get => Get();
        set
        {
            Set(value);
            LocalPath = "/Source/Presentation/Resources/Images/1.png";
        }
    }
    public string LocalPath { get => Get(); set => Set(value); }
}
