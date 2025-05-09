namespace Parseh.UI.ViewModels;

public sealed class ChatViewModel : VM
{
    public ChatContactViewModel Contact { get => Get(); private set => Set(value); }

    public ChatViewModel()
    {
        // TODO: Load data form server (only Contats-Items Info)
    }
}

public sealed class ChatContactViewModel : VM
{
    public ObservableSet<ChatContactItemViewModel> Contacts { get => Get(); private set => Set(value); }

    public ChatContactViewModel()
    {
        //Contacts = [];
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