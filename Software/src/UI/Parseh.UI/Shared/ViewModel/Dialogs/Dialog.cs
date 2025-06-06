namespace Parseh.UI.Views;

using System.ComponentModel;

public abstract class Dialog : Component
{
    #region Properties

    protected Notifier Notifier { get; private set; } = default!;

    #endregion

    #region Commands

    public IRelayCommand CloseCommand { get; private set; } = default!;

    #endregion

    protected Dialog()
    {
        if (!DesignerProperties.GetIsInDesignMode(this))
        {
            Notifier = new();
            Notifier.Owner = App.Layout;
            Notifier.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            CloseCommand = new Command(Close);
        }
    }

    public Task NotifyAsync<T>(T viewmodel)
        where T : DialogMessageViewModel
    {
        var tcs = new TaskCompletionSource<bool>();
        App.Dispatch(() =>
        {
            try
            {
                DataContext = viewmodel;
                Notifier.Model.Type = viewmodel.Type;
                Notifier.DialogContnet = this;
                Notifier.ShowDialog();
            }
            finally
            {
                tcs.SetResult(true);
            }
        });
        return tcs.Task;
    }

    void Close() => Notifier.Model.CloseCommand.Execute(null);
}