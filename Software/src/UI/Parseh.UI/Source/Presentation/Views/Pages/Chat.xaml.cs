namespace Parseh.UI.Views;

public partial class Chat : ContentPage<ChatViewModel>
{
    public Chat(ChatViewModel model) : base(model)
        => Init();

    private void Init()
        => InitializeComponent();
}


