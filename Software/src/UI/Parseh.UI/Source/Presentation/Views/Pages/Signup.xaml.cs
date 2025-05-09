namespace Parseh.UI.Views;

public partial class Signup : ContentPage<SignupViewModel>
{
    public Signup() => InitializeComponent();

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Generic.Self.ViewModel.ToPage(PageMode.Signin);
    }
}
