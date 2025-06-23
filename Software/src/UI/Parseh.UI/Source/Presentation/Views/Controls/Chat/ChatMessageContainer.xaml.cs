namespace Parseh.UI.Views;

public partial class ChatMessageContainer : Component
{
    public ChatMessageContainer() => InitializeComponent();

    void OnSendButton(object sender, RoutedEventArgs e) => Message.Focus();

    void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                var index = Message.CaretIndex;
                Message.Text = Message.Text.Insert(index, Environment.NewLine);
                Message.CaretIndex = index + Environment.NewLine.Length;
            }
            else
            {
                if (!Message.Text.IsEmpty())
                    DataContext.As<ChatViewModel>().SendMessageCommand.Execute(null);
            }
            e.Handled = true;
        }
    }
}

// TODO: وضعیت نمایش منو اتچمنت درست شود.
// وقتی موس از روی دکمه اتچمن برداشته می شود، نباید منو آن حذف شود
// تا بتوانیم روی آن کلیک کنیم
