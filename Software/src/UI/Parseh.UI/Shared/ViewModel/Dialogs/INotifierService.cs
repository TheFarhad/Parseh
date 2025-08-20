namespace Parseh.UI.ViewModels;

public interface INotifierService
{
    Task MessageboxNotifyAsync(DialogMessageViewModel message);
    // TODO: متناسب با دیالوگ های مختلف، متود های مختلفی هم نوشته شود که هر کدام ویومدل خودشان را دارند و همینطور دیالوگ اختصاصی خودشان را فراخوانی می کنند
}

public sealed class NotifierService : INotifierService
{
    public Task MessageboxNotifyAsync(DialogMessageViewModel message)
    {
        var tcs = new TaskCompletionSource<bool>();

        App.DispatchAsync(async () =>
        {
            try
            {
                await new Views.MessageBox().NotifyAsync(message);
            }
            finally
            {
                tcs.SetResult(true);
            }
        });

        return tcs.Task;
    }
}
