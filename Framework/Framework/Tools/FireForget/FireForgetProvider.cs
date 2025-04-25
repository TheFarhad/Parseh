namespace Framework;

using Microsoft.Extensions.DependencyInjection;

public class FireForgetProvider(IServiceScopeFactory serviceScopeFactory)
{
    private readonly IServiceScopeFactory _serviceScope = serviceScopeFactory;

    public void Execute<TService>(Func<TService, Task> func, Action<Exception> onFailure = default!)
    {
        Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceScope.CreateScope();
                var service = scope.ServiceProvider.GetService<TService>();
                if (service is { }) await func(service);

            }
            catch (Exception e)
            {
                if (true) onFailure.Invoke(e);
                else
                {
                    // customize exception...
                    Console.WriteLine(e.Message);
                }
            }
        });
    }
}