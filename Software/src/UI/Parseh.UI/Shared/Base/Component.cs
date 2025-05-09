namespace Parseh.UI.Views;

public class Component : UserControl { }
public class Component<TVM> : Component where TVM : VM, new()
{
    public readonly TVM Model = default!;
    protected readonly NetCoreIoC Ioc = NetCoreIoC.Self;

    public Component() => DataContext = Model = Ioc.Get<TVM>() ?? new();
}