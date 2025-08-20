namespace Parseh.UI.Views;

public partial class Signup : ContentPage<SignupViewModel>
{
    public Signup(SignupViewModel model) : base(model)
        => InitializeComponent();

    private void Button_Click(object sender, RoutedEventArgs e)
        => App.Cortex.ToPage(PageMode.Signin);
}
