namespace Parseh.UI.ViewModels;

using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Diagnostics.Contracts;

public sealed class ChatViewModel : VM
{
    #region Properties

    public double SettingbarHeight { get => Get(); private set => Set(value); }
    //public bool SelectedChat { get => Get(); private set => Set(value); }
    public bool IsSearching { get => Get(); private set => Set(value); }
    public bool IsOpenAttachmentMenu { get => Get(); private set => Set(value); }
    public bool IsOpenSettingMenu { get => Get(); private set => Set(value); }
    public ObservableSet<ChatContactViewModel> Contacts { get => Get(); private set => Set(value); }
    public AttachmentMenuViewModel AttachmentMenuModel { get => Get(); private set => Set(value); }
    public SettingMenuViewModel SettingMenuModel { get => Get(); private set => Set(value); }

    // TODO: دیتاکانتکست مربوط پیام ها هم از این آیتم گرفته می شود و به یوزرکنترل زیر داده می شود
    // ChatMessageContainerCard
    //public ChatContactViewModel SelectedContact => Contacts.Single(_ => _.Selected);
    public ChatContactViewModel? SelectedContact { get => Get(); private set => Set(value!); }
    public bool SelectedChat => Contacts.Any(_ => _.Selected);

    #endregion

    #region Commands

    public IRelayCommand ShowSearchbarCommand { get; private set; } = default!;
    public IRelayCommand CloseSearchbarCommand { get; private set; } = default!;
    public IRelayCommand LockCommand { get; private set; } = default!;
    public IRelayCommand ToggleSettingMenuCommand { get; private set; } = default!;
    public IRelayCommand ToggleAttachmentMenuCommand { get; private set; } = default!;

    #endregion

    public ChatViewModel() => Init();

    #region Private Functionality

    void Init()
    {
        InitProperties();
        InitCommands();
    }

    void InitProperties()
    {
        SettingbarHeight = 60;
        IsSearching = false;
        IsOpenAttachmentMenu = false;
        IsOpenSettingMenu = false;
        AttachmentMenuModel = new();

        // TODO: Load data form server (only Contats-Items Info)
        Contacts = [
            new ChatContactViewModel
        {
            Id= 1,
            Nikname = "PS",
            Contact = "Panah",
            Message = "Hi, Where are you?!!",
            Selected = false,
            Pinned = true,
            UnreadMessageCount = 1007,
            Messages = [
                new ()
            {
                SendByMe = true,
                Sender = "Ali",
                Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                SendAt = DateTime.UtcNow,
                ReadAt = DateTime.UtcNow.AddMinutes(10)
            },
            new() {
                SendByMe = false,
                Sender = "Panah",
                Message = "Hi Ali. How are you?",
                SendAt = DateTime.UtcNow.AddMinutes(1),
                ReadAt = DateTime.UtcNow.AddMinutes(5)
            },
            new() {
                SendByMe = false,
                Sender = "Panah",
                Message = "Hi Ali. How are you?",
                SendAt = DateTime.UtcNow.AddMinutes(1),
                ReadAt = DateTime.UtcNow.AddMinutes(5)
            },
            new() {
                SendByMe = false,
                Sender = "Panah",
                Message = "Hi Ali. How are you?",
                SendAt = DateTime.UtcNow.AddMinutes(1),
                ReadAt = DateTime.UtcNow.AddMinutes(5)
            }
                ]
        },
        new ChatContactViewModel
        {
            Id= 2,
            Nikname = "FK",
            Contact = "Farshid",
            Message = "Hi, Where are you?!!",
            Selected = false,
            Pinned = false,
            UnreadMessageCount = 3,
             Messages = [
                new ()
            {
                SendByMe = true,
                Sender = "Ali",
                Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                SendAt = DateTime.UtcNow,
                ReadAt = DateTime.UtcNow.AddMinutes(10)
            },
            new() {
                SendByMe = false,
                Sender = "Panah",
                Message = "Hi Ali. How are you?",
                SendAt = DateTime.UtcNow.AddMinutes(1),
                ReadAt = DateTime.UtcNow.AddMinutes(5)
            }
            ]
        },
        new ChatContactViewModel
        {
            Id= 3,
            Nikname = "FK",
            Contact = "Farshid",
            Message = "Hi, Where are you?!! Where are you?!! Where are you?!!",
            Selected = false,
            Pinned = false,
            UnreadMessageCount = 9,
             Messages = [
                new ()
            {
                SendByMe = true,
                Sender = "Ali",
                Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                SendAt = DateTime.UtcNow,
                ReadAt = DateTime.UtcNow.AddMinutes(10)
            }]
        }
            ];

        SelectedContact = new();

        MonitorContacts();

        // TODO:  SettingMenuModel = Cortex.Default.Model.SettingMenuModel;
        SettingMenuModel = new();
    }

    void MonitorContacts()
        => Contacts
            .ToList()
            .ForEach(contact => contact.PropertyChanged += OnContactPropertyChanged);

    void OnContactPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName?.Equals(nameof(ChatContactViewModel.Selected)) is true)
            ChangeSelectedContact(sender!.As<ChatContactViewModel>());
    }

    void ChangeSelectedContact(ChatContactViewModel contact)
    {
        if (contact.Selected)
        {
            SelectedContact = contact;
            Notify(nameof(SelectedChat));

            var lastSelectedContact = Contacts.Where(_ => _.Selected && _.Id != SelectedContact!.Id).SingleOrDefault();
            if (lastSelectedContact is not null)
            {
                var index = Contacts.IndexOf(lastSelectedContact);
                Contacts[index].Selected = false;
            }
        }
    }

    void InitCommands()
    {
        ShowSearchbarCommand = new Command(ShowSearchbar);
        CloseSearchbarCommand = new Command(CloseSearchbar);
        LockCommand = new Command(Lock);
        ToggleSettingMenuCommand = new Command(ToggleSettingMenu);
        ToggleAttachmentMenuCommand = new Command(ToggleAttachmentMenu);
    }

    #endregion

    #region Command Methds

    void ShowSearchbar() => IsSearching = true;
    void CloseSearchbar() => IsSearching = false;
    void Lock() => Cortex.Default.Model.ToPage(PageMode.Signin);
    void ToggleSettingMenu()
    {
        IsOpenSettingMenu ^= true;
        SettingMenuUnedittedMode();

    }
    void ToggleAttachmentMenu() => IsOpenAttachmentMenu ^= true;

    void SettingMenuUnedittedMode()
    {
        SettingMenuModel.Name.IsEditing = false;
        SettingMenuModel.Email.IsEditing = false;
        SettingMenuModel.Passcode.IsEditing = false;
    }

    #endregion
}

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
        Messages = [
             new ()
            {
                SendByMe = true,
                Sender = "Ali",
                Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                SendAt = DateTime.UtcNow,
                ReadAt = DateTime.UtcNow.AddMinutes(10)
            },
            new() {
                SendByMe = false,
                Sender = "Panah",
                Message = "Hi Ali. How are you?",
                SendAt = DateTime.UtcNow.AddMinutes(1),
                ReadAt = DateTime.UtcNow.AddMinutes(5)
            },
            new() {
                SendByMe = false,
                Sender = "Panah",
                Message = "Hi Ali. How are you?",
                SendAt = DateTime.UtcNow.AddMinutes(1),
                ReadAt = DateTime.UtcNow.AddMinutes(5)
            },
            new() {
                SendByMe = false,
                Sender = "Panah",
                Message = "Hi Ali. How are you?",
                SendAt = DateTime.UtcNow.AddMinutes(1),
                ReadAt = DateTime.UtcNow.AddMinutes(5)
            }
            ];
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