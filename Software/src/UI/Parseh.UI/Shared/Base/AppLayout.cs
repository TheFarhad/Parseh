namespace Parseh.UI.Views;

public class AppLayout : Window { }
public class Dialog : AppLayout { }


public class ContentPage : Page { }
public class ContentPage<TViewmodel> : ContentPage where TViewmodel : VM, new()
{
    protected readonly TViewmodel Model = default!;

    public ContentPage() => DataContext = Model = new();
}

public class ContentView : UserControl { }
public class ContentView<TViewmodel> : ContentView where TViewmodel : VM, new()
{
    protected readonly TViewmodel Model = default!;

    public ContentView() => DataContext = Model = new();
}