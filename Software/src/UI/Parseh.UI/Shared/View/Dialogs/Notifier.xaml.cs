namespace Parseh.UI.Views;

public partial class Notifier : ContentLayout
{
    public NotifierViewModel Model { get; set; } = default!;

    public static DependencyProperty DialogContnetProperty =
        DependencyProperty
            .Register(nameof(DialogContnet),
            typeof(Control),
            typeof(Notifier),
            new UIPropertyMetadata(default(Control), OnDialogContnetChanged, null));

    public Control DialogContnet
    {
        get => GetValue(DialogContnetProperty).As<Control>();
        set => SetValue(DialogContnetProperty, value);
    }

    public Notifier() => Init();

    void Init()
    {
        Model = new NotifierViewModel(this);
        InitializeComponent();
    }

    void OnAtcivated(object sender, EventArgs e) => Model.LayoutActivationCommand.Execute(null);

    void OnDeactivated(object sender, EventArgs e) => Model.LayoutActivationCommand.Execute(null);

    static void OnDialogContnetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }
}
