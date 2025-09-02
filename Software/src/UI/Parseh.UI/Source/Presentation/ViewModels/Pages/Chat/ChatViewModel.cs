namespace Parseh.UI.ViewModels;

using System.ComponentModel;

public sealed class ChatViewModel : ViewModel
{
    private readonly IServiceProvider _serviceProvider;

    #region Properties

    public double SettingbarHeight { get => Get(); private set => Set(value); }
    public bool IsSearching { get => Get(); private set => Set(value); }
    public bool IsOpenAttachmentMenu { get => Get(); set => Set(value); }
    public bool IsOpenSettingMenu { get => Get(); private set => Set(value); }
    public bool IsShowGoToBottomButton { get => Get(); private set => Set(value); }
    public MvvmSet<ContactChatViewModel> Contacts { get => Get(); private set => Set(value); }
    public AttachmentMenuViewModel AttachmentMenu { get => Get(); private set => Set(value); }
    public ProfileViewModel Profile { get => Get(); private set => Set(value); }
    public string Message { get => Get(); set => Set(value); }
    //public ChatContactViewModel? SelectedContact { get => Get(); private set => Set(value); }
    public ContactChatViewModel SelectedContact { get => Get(); private set => Set(value); }
    //public bool SelectedChat => Contacts.Any(_ => _.Selected);
    public bool SelectedChat => SelectedContact.Selected;
    public string SearchText { get => Get(); set => Set(value); }

    #endregion

    #region Commands

    public IRelayCommand ShowSearchbarCommand { get; private set; } = default!;
    public IRelayCommand CloseSearchbarCommand { get; private set; } = default!;
    public IRelayCommand LockCommand { get; private set; } = default!;
    public IRelayCommand ToggleSettingMenuCommand { get; private set; } = default!;
    public IRelayCommand ToggleAttachmentMenuCommand { get; private set; } = default!;
    public IRelayCommand SendMessageCommand { get; private set; } = default!;
    public IRelayCommand SearchCommand { get; private set; } = default!;
    public IRelayCommand SelectContactCommand { get; private set; } = default!;

    #endregion

    public ChatViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Init();
    }

    #region Private Functionality

    private void Init()
    {
        InitProperties();
        InitCommands();
    }

    private void InitProperties()
    {
        SettingbarHeight = 60;
        IsSearching = false;
        IsOpenAttachmentMenu = false;
        IsOpenSettingMenu = false;
        IsShowGoToBottomButton = false;
        AttachmentMenu = new();
        SearchText = Empty;

        // TODO: Load data form server (only Contats-Items Info)
        // Contects = GetDataFromServer() ?? [];
        Contacts = [
                 new ContactChatViewModel
                 {
                    Id= 1,
                    Nikname = "PS",
                    Contact = "Panah",
                    //Message = "Hi, Where are you?!!",
                    Selected = false,
                    Pinned = true,
                    UnreadMessageCount = 0,
                    Messages = [
                        new()
                        {
                            SendByMe = true,
                            Sender = "Ali",
                            Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                            SendAt = DateTime.UtcNow,
                            ReadAt = DateTime.UtcNow.AddMinutes(2),
                            Image = new()
                                {
                                 Title = "XYZ",
                                 FileName= "1.png",
                                 Size = 12365479,
                                 LocalPath = "/Source/Presentation/Resources/Images/1.png"
                                }
                        },
                        new()
                        {
                            SendByMe = true,
                            Sender = "Ali",
                            Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                            SendAt = DateTime.UtcNow,
                            ReadAt = DateTime.UtcNow.AddMinutes(2)
                        },
                        new() {
                            SendByMe = false,
                            Sender = "Panah",
                            Message = "Hi Ali. How are you?",
                            SendAt = DateTime.UtcNow.AddMinutes(1),
                            ReadAt = DateTime.UtcNow.AddMinutes(3)
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
                            ReadAt = DateTime.UtcNow.AddMinutes(15)
                        },
                        new()
                            {
                            SendByMe = true,
                            Sender = "Ali",
                            Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                            SendAt = DateTime.UtcNow,
                            ReadAt = DateTime.UtcNow.AddMinutes(2)
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
                            ReadAt = DateTime.UtcNow.AddMinutes(15)
                        }
                        ]
                 },
                 new ContactChatViewModel
                 {
                     Id= 2,
                     Nikname = "FK",
                     Contact = "Farshid",
                     //Message = "Hi, Where are you?!!",
                     Selected = false,
                     Pinned = false,
                     UnreadMessageCount = 3,
                     Messages = [
                         new()
                         {
                             SendByMe = true,
                             Sender = "Ali",
                             Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                             SendAt = DateTime.UtcNow,
                             ReadAt = DateTime.UtcNow.AddMinutes(10)
                         },
                         new()
                         {
                            SendByMe = false,
                            Sender = "Panah",
                            Message = "Hi Ali. How are you?",
                            SendAt = DateTime.UtcNow.AddMinutes(1),
                            ReadAt = DateTime.UtcNow.AddMinutes(5)
                         }
                     ]
                 },
                 new ContactChatViewModel
                {
                    Id= 3,
                    Nikname = "FK",
                    Contact = "Farshid",
                    //Message = "Hi, Where are you?!! Where are you?!! Where are you?!!",
                    Selected = false,
                    Pinned = false,
                    UnreadMessageCount = 12,
                    Messages = [
                        new()
                    {
                        SendByMe = true,
                        Sender = "Ali",
                        Message = "",
                        SendAt = DateTime.UtcNow,
                        ReadAt = DateTime.UtcNow.AddMinutes(10)
                    },
                        new()
                        {
                        SendByMe = true,
                        Sender = "Ali",
                        Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                        SendAt = DateTime.UtcNow,
                        ReadAt = DateTime.UtcNow.AddMinutes(2)
                        },
                        new() {
                            SendByMe = false,
                            Sender = "Panah",
                            Message = "Hi Ali. How are you?",
                            SendAt = DateTime.UtcNow.AddMinutes(1),
                            ReadAt = DateTime.UtcNow.AddMinutes(3)
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
                            ReadAt = DateTime.UtcNow.AddMinutes(15)
                        },
                        new()
                            {
                            SendByMe = true,
                            Sender = "Ali",
                            Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                            SendAt = DateTime.UtcNow,
                            ReadAt = DateTime.UtcNow.AddMinutes(2)
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
                            ReadAt = DateTime.UtcNow.AddMinutes(15)
                        },
                        new()
                            {
                            SendByMe = true,
                            Sender = "Ali",
                            Message = "Binding Path=Width, RelativeSource={RelativeSource Mode=Self =Width, RelativeSource={RelativeSource Mode=Se =Width, RelativeSource={RelativeSource Mode=Se",
                            SendAt = DateTime.UtcNow,
                            ReadAt = DateTime.UtcNow.AddMinutes(2)
                            },
                        new() {
                            SendByMe = false,
                            Sender = "Panah",
                            Message = "Hi Ali. How are you?",
                            SendAt = DateTime.UtcNow.AddMinutes(1),
                            ReadAt = DateTime.UtcNow.AddMinutes(5)
                        }
                        ]
                 }
            ];

        SelectedContact = new();

        //MonitorContacts();

        Profile = App.Cortex.SettingMenuModel;
    }

    private void InitCommands()
    {
        ShowSearchbarCommand = new Command(ShowSearchbar);
        CloseSearchbarCommand = new Command(CloseSearchbar);
        LockCommand = new Command(Lock);
        ToggleSettingMenuCommand = new Command(ToggleSettingMenu);
        ToggleAttachmentMenuCommand = new Command(ToggleAttachmentMenu);
        SendMessageCommand = new Command(SendMessage);
        SearchCommand = new Command(Search);


        SelectContactCommand = new Command<ContactChatViewModel>(SelectContatc);
    }

    private void MonitorContacts()
        => Contacts
            .ToList()
            .ForEach(contact => contact.PropertyChanged += OnContactPropertyChanged);

    private void OnContactPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName?.Equals(nameof(ContactChatViewModel.Selected)) is true)
            ChangeSelectedContact(sender!.As<ContactChatViewModel>());
    }

    private void ChangeSelectedContact(ContactChatViewModel contact)
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

    #endregion

    #region Command Methds

    private void ShowSearchbar()
        => IsSearching = true;

    private void CloseSearchbar()
    {
        IsSearching = false;
        SearchText = Empty;
        Search();
    }

    private void Lock()
        => App.Cortex.ToPage(PageMode.Signin);

    private void ToggleSettingMenu()
    {
        IsOpenSettingMenu ^= true;
        SettingMenuUnedittedMode();

    }

    private void ToggleAttachmentMenu() => IsOpenAttachmentMenu ^= true;

    private void SettingMenuUnedittedMode()
    {
        Profile.Name.IsEditing = false;
        Profile.Email.IsEditing = false;
        Profile.Passcode.IsEditing = false;
    }

    private void SendMessage()
    {
        if (Message.IsEmpty()) return;

        if (SelectedContact is null) SelectedContact = new();

        // TODO: Save Message To Server
        // TODO: If Ok, Then
        var message = new ContactChatMessageViewModel
        {
            SendByMe = true,
            Sender = "Ali",
            Message = Message,
            SendAt = DateTime.UtcNow,
            IsNewMessage = true
        };
        SelectedContact.Messages.Add(message);
        Search();

        Message = Empty;
    }

    private void Search()
        => SelectedContact.Search(SearchText);

    private void SelectContatc(ContactChatViewModel source)
    {
        SelectedContact = source;
        SelectedContact.Select();
        Notify(nameof(SelectedChat));

        var lastSelectedContact = Contacts.Where(_ => _.Selected && _.Id != SelectedContact!.Id).SingleOrDefault();
        if (lastSelectedContact is not null)
        {
            var index = Contacts.IndexOf(lastSelectedContact);
            Contacts[index].Selected = false;
        }
    }

    #endregion
}

