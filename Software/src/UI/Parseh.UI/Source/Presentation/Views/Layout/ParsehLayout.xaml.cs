namespace Parseh.UI;

public partial class ParsehLayout : Layout
{
    internal readonly LayoutViewModel ViewModel;

    public ParsehLayout()
    {
        InitializeComponent();
        ViewModel = new LayoutViewModel(this);

        // TODO: remove finally
        DataContext = ViewModel;
    }
}