﻿namespace Parseh.UI.Views;

public partial class Signup : ContentPage<SignupViewModel>
{
    public Signup() => InitializeComponent();

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Cortex.Default.Model.ToPage(PageMode.Signin);
    }
}
