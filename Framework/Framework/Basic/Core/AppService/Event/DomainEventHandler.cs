namespace Framework;

public interface IDomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken? token = null);
}

public sealed class DomainEventPipe(IServiceProvider serviceProvider) : Pipe<DomainEventPipe>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task HandleAsync<TDomainEvent>(TDomainEvent source, CancellationToken? token = null)
        where TDomainEvent : IDomainEvent
    {
        List<Task> tasks = [];
        _serviceProvider
              .GetServices<IDomainEventHandler<TDomainEvent>>()
              .ToList()
              .ForEach(handler => tasks.Add(handler.HandleAsync(source, token)));
        await Task.WhenAll(tasks);
    }
}