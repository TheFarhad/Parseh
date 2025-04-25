namespace Parseh.UI;

public partial class Layout : Window { }
public partial class Dialog : Layout { }

public abstract class Page<TViewmodel> : Page where TViewmodel : VM
{
    public readonly TViewmodel ViewModel;

    public Page(TViewmodel viewmodel) => ViewModel = viewmodel;
}

public abstract class Control<TViewmodel> : UserControl where TViewmodel : VM
{
    public readonly TViewmodel ViewModel;

    public Control(TViewmodel viewmodel) => ViewModel = viewmodel;
}