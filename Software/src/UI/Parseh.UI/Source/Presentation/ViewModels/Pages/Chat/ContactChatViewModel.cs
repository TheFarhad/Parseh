namespace Parseh.UI.ViewModels;

public sealed class ContactChatViewModel : ViewModel
{
    #region Properties

    public int Id { get => Get(); set => Set(value); }
    public string Nikname { get => Get(); set => Set(value); }
    public string Contact { get => Get(); set => Set(value); }
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
    public MvvmSet<ContactChatMessageViewModel> Messages
    {
        get => Get(); set
        {
            Set(value);
            Notify(nameof(Message));
        }
    }
    public MvvmSet<ContactChatMessageViewModel> SearchMessages { get => Get(); private set => Set(value); }
    public string Message => Messages?.Any() is true ? Messages.Last().Message : Empty;
    public bool ShowChevronDownButton { get => Get(); set => Set(value); }
    public bool ScrollToLastMessage { get => Get(); set => Set(value); }

    #endregion

    #region Commands

    public IRelayCommand SelectCommand { get; private set; } = default!;
    public IRelayCommand ScrollToBottomCommand { get; private set; } = default!;

    #endregion

    public ContactChatViewModel() => Init();

    #region Public Functionality

    public void Select()
    {
        Selected = true;
        SearchMessages = new MvvmSet<ContactChatMessageViewModel>(Messages);

        // TODO: لود کردن تعداد مشخصی از پیامها از آخر
        // مثلا 20 پیام آخر لود شود 
        // سپس با هر بال اسکرول کرن به بالا، همین دعداد مجددا لود شود
    }

    public void Search(string filter)
    {
        var filterdMessages = Messages.Where(_ => _.Message.Contains(filter));
        SearchMessages = new MvvmSet<ContactChatMessageViewModel>(filterdMessages);
    }

    #endregion

    #region Private Functionality

    void Init()
    {
        InitProperties();
        InitComands();
    }

    void InitProperties()
    {
        // TODO: get form server: Data ?? [];
        Messages = [];
        SearchMessages = [];
        LastSeen = DateTime.Now;
        ShowChevronDownButton = false;
        ScrollToLastMessage = false;
        Selected = false;
    }

    void InitComands()
    {
        SelectCommand = new Command(Select);
        ScrollToBottomCommand = new Command(ScrollToBottom);
    }

    void ScrollToBottom() => ScrollToLastMessage = true;

    #endregion
}


#region Design Models

public sealed class ChatContactItemDesignModel1 : DesignModel<ChatContactItemDesignModel1, ContactChatViewModel>
{
    public ChatContactItemDesignModel1()
    {
        Model.Nikname = "P";
        Model.Contact = "Panah";
        //Model.Message = "Hi, Where are you?!!";
        Model.Selected = true;
        Model.UnreadMessageCount = 7;
        Model.Pinned = true;
    }
}

#endregion