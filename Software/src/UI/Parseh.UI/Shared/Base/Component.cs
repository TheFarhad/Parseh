namespace Parseh.UI.Views;

public class Component : UserControl
{
    //public readonly Ioc Ioc = Ioc.Default;
}
public class Component<TViewmodel> : Component where TViewmodel : ViewModel, new()
{
    public readonly TViewmodel Model = default!;

    public Component() => DataContext = Model = App.RequiredService<TViewmodel>() ?? new();
}