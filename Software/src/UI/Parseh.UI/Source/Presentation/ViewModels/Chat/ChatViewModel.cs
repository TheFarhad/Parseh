﻿namespace Parseh.UI.ViewModels;

public sealed class ChatViewModel : VM
{
    #region Properties

    public ChatContactViewModel Contact { get => Get(); private set => Set(value); }
    public double SettingbarHeight { get => Get(); private set => Set(value); }
    public bool SelectedChat { get => Get(); private set => Set(value); }
    public bool IsSearching { get => Get(); private set => Set(value); }
    public bool IsOpenAttachmentMenu { get => Get(); private set => Set(value); }
    public bool IsOpenSettingMenu { get => Get(); private set => Set(value); }

    #endregion

    #region Commands

    public IRelayCommand ShowSearchbarCommand { get; private set; } = default!;
    public IRelayCommand CloseSearchbarCommand { get; private set; } = default!;
    public IRelayCommand LockCommand { get; private set; } = default!;
    public IRelayCommand ToggleSettingMenuCommand { get; private set; } = default!;
    public IRelayCommand ToggleAttachmentMenuCommand { get; private set; } = default!;

    #endregion

    public ChatViewModel()
    {
        Init();
        // TODO: Load data form server (only Contats-Items Info)
    }

    void Init()
    {
        InitProperties();
        InitCommands();
    }

    void InitProperties()
    {
        SettingbarHeight = 60;
        IsSearching = false;
        Contact = new();
        SelectedChat = true;
        IsOpenAttachmentMenu = false;
        IsOpenSettingMenu = false;
    }

    void InitCommands()
    {
        ShowSearchbarCommand = new Command(ShowSearchbar);
        CloseSearchbarCommand = new Command(CloseSearchbar);
        LockCommand = new Command(Lock);
        ToggleSettingMenuCommand = new Command(ToggleSettingMenu);
        ToggleAttachmentMenuCommand = new Command(ToggleAttachmentMenu);
    }

    #region Private Functionality

    void ShowSearchbar() => IsSearching = true;
    void CloseSearchbar() => IsSearching = false;
    void Lock() => Cortex.Default.Model.ToPage(PageMode.Signin);
    void ToggleSettingMenu() => IsOpenSettingMenu ^= true;
    void ToggleAttachmentMenu() => IsOpenAttachmentMenu ^= true;

    #endregion
}

public sealed class ChatContactViewModel : VM
{
    public ObservableSet<ChatContactItemViewModel> Contacts { get => Get(); private set => Set(value); }

    public ChatContactViewModel()
    {
        Contacts.Add(new ChatContactItemViewModel
        {
            Nikname = "PS",
            Contact = "Panah",
            Message = "Hi, Where are you?!!",
            Selected = false,
            Pinned = true,
            UnreadMessageCount = 1007
        });
        Contacts.Add(new ChatContactItemViewModel
        {
            Nikname = "FK",
            Contact = "Farshid",
            Message = "Hi, Where are you?!!",
            Selected = true,
            Pinned = false,
            UnreadMessageCount = 3
        });
        Contacts.Add(new ChatContactItemViewModel
        {
            Nikname = "FK",
            Contact = "Farshid",
            Message = "Hi, Where are you?!! Where are you?!! Where are you?!!",
            Selected = false,
            Pinned = false,
            UnreadMessageCount = 9
        });
    }
}



public sealed class ChatContactItemViewModel : VM
{
    public string Nikname { get => Get(); set => Set(value); }
    public string Contact { get => Get(); set => Set(value); }
    public string Message { get => Get(); set => Set(value); }
    public bool Selected { get => Get(); set => Set(value); }
    public bool Pinned { get => Get(); set => Set(value); }
    public int UnreadMessageCount { get => Get(); set => Set(value); }
}

#region Design Models

public sealed class ChatContactDesignModel : DesignModel<ChatContactDesignModel, ChatContactViewModel>
{
    public ChatContactDesignModel()
    {
        Model.Contacts.Add(new ChatContactItemViewModel
        {
            Nikname = "PS",
            Contact = "Panah",
            Message = "Hi, Where are you?!!",
            Selected = true,
            Pinned = true,
            UnreadMessageCount = 1007
        });
        Model.Contacts.Add(new ChatContactItemViewModel
        {
            Nikname = "FK",
            Contact = "Farshid",
            Message = "Hi, Where are you?!!",
            Selected = false,
            Pinned = false,
            UnreadMessageCount = 3
        });
        //Model.Contacts.Add(new ChatContactItemViewModel
        //{
        //    Nikname = "MK",
        //    Contact = "Mehri",
        //    Message = "Hi, Where are you?!!",
        //    Selected = true,
        //    Pinned = false,
        //    UnreadMessageCount = 16
        //});
        //Model.Contacts.Add(new ChatContactItemViewModel
        //{
        //    Nikname = "MK",
        //    Contact = "Majid",
        //    Message = "Hi, Where are you?!!",
        //    Selected = false,
        //    Pinned = false,
        //    UnreadMessageCount = 23
        //});
        //Model.Contacts.Add(new ChatContactItemViewModel
        //{
        //    Nikname = "AA",
        //    Contact = "Abbas",
        //    Message = "Hi, Where are you?!!",
        //    Selected = false,
        //    Pinned = true,
        //    UnreadMessageCount = 2
        //});
        //Model.Contacts.Add(new ChatContactItemViewModel
        //{
        //    Nikname = "AA",
        //    Contact = "Abbas",
        //    Message = "Hi, Where are you?!!",
        //    Selected = false,
        //    Pinned = false,
        //    UnreadMessageCount = 37
        //});
        //Model.Contacts.Add(new ChatContactItemViewModel
        //{
        //    Nikname = "AA",
        //    Contact = "Abbas",
        //    Message = "Hi, Where are you?!!",
        //    Selected = false,
        //    Pinned = false,
        //    UnreadMessageCount = 42
        //});
        //Model.Contacts.Add(new ChatContactItemViewModel
        //{
        //    Nikname = "AA",
        //    Contact = "Abbas",
        //    Message = "Hi, Where are you?!!",
        //    Selected = false,
        //    Pinned = false,
        //    UnreadMessageCount = 5
        //});

        //Model.Contacts.OrderBy(_ => _.Pinned);
    }
}

public sealed class ChatContactItemDesignModel1 : DesignModel<ChatContactItemDesignModel1, ChatContactItemViewModel>
{
    public ChatContactItemDesignModel1()
    {
        Model.Nikname = "P";
        Model.Contact = "Panah";
        Model.Message = "Hi, Where are you?!!";
        Model.Selected = true;
        Model.UnreadMessageCount = 7;
    }
}

#endregion