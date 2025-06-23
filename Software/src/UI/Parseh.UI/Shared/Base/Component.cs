namespace Parseh.UI.Views;

public class Component : UserControl
{
    public readonly Ioc Ioc = Ioc.Default;
}
public class Component<TVM> : Component where TVM : VM, new()
{
    public readonly TVM Model = default!;

    public Component() => DataContext = Model = Ioc.Service<TVM>() ?? new();
}