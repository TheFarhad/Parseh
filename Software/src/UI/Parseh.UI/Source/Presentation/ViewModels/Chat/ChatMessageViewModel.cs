namespace Parseh.UI.ViewModels;

public sealed class ChatMessageViewModel : VM
{
    public string Sender { get => Get(); set => Set(value); }
    public string Message { get => Get(); set => Set(value); }
    public DateTimeOffset SendAt { get => Get(); set => Set(value); }
    public DateTimeOffset ReadAt { get => Get(); set => Set(value); }
    public bool SendByMe { get => Get(); set => Set(value); }

    public ChatMessageViewModel()
    {
        SendByMe = true;
        Sender = "Ali";
        Message = "Hi everybody. Chat Message Desig";
        SendAt = DateTime.UtcNow;
        ReadAt = DateTime.UtcNow.AddMinutes(10);
    }
}

public sealed class ChatMessagesViewModel : VM
{
    public ObservableSet<ChatMessageViewModel> Messages { get => Get(); set => Set(value); }
}


public sealed class ChatMessagesDesignModel : DesignModel<ChatMessagesDesignModel, ChatMessagesViewModel>
{
    public ChatMessagesDesignModel()
    {
        Model.Messages = new ObservableSet<ChatMessageViewModel>
        {
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
        };
    }
}


public sealed class ChatMessageDesignModel : DesignModel<ChatMessageDesignModel, ChatMessageViewModel>
{
    public ChatMessageDesignModel()
    {
        Model.SendByMe = true;
        Model.Sender = "Ali";
        Model.Message = "Hi everybody. Chat Message Designodel";
        Model.SendAt = DateTime.UtcNow;
        Model.ReadAt = DateTime.UtcNow.AddMinutes(10);
    }
}