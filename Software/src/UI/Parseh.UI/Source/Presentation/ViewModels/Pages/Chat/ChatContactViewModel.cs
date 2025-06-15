namespace Parseh.UI.ViewModels;

public sealed class ChatContactViewModel : VM
{
    #region Properties

    public int Id { get => Get(); set => Set(value); }
    public string Nikname { get => Get(); set => Set(value); }
    public string Contact { get => Get(); set => Set(value); }
    public string Message { get => Get(); set => Set(value); }
    public bool Selected { get => Get(); set => Set(value); }
    public bool Pinned { get => Get(); set => Set(value); }
    public DateTime LastSeen { get => Get(); set => Set(value); } // TODO: یک کانورتر برای نمایش دلخواه زمان آخرین بازدید نوشته شود
    public int UnreadMessageCount
    {
        get => Get();
        set
        {
            Set(value);
            Notify(nameof(HaveUnreadMessages));
        }
    }
    public bool HaveUnreadMessages => UnreadMessageCount > 0;
    public ObservableSet<ChatMessageViewModel> Messages { get => Get(); set => Set(value); }

    #endregion

    #region Properties

    public IRelayCommand SelectCommand { get; private set; } = default!;

    #endregion

    public ChatContactViewModel() => Init();

    void Init()
    {
        InitProperties();
        InitComands();
    }

    void InitProperties()
    {
        LastSeen = DateTime.Now;
    }

    void InitComands()
    {
        SelectCommand = new Command(Select);
    }

    void Select()
    {
        Selected = true;

        // TODO: لود کردن تعداد مشخصی از پیامها از آخر
        // مثلا 20 پیام آخر لود شود 
        // سپس با هر بال اسکرول کرن به بالا، همین دعداد مجددا لود شود
    }
}


#region Design Models

public sealed class ChatContactItemDesignModel1 : DesignModel<ChatContactItemDesignModel1, ChatContactViewModel>
{
    public ChatContactItemDesignModel1()
    {
        Model.Nikname = "P";
        Model.Contact = "Panah";
        Model.Message = "Hi, Where are you?!!";
        Model.Selected = true;
        Model.UnreadMessageCount = 7;
        Model.Pinned = true;
    }
}

#endregion