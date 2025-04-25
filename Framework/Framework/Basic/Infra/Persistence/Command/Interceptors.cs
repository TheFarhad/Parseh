namespace Framework;

using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class SaveInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> interceptionResult, CancellationToken token = default!)
    {
        ValueTask<InterceptionResult<int>> result = default!;
        var context = GetContext(eventData);
        context.ChangeTracker.DetectChanges();
        BeforeSaving(context);
        context.ChangeTracker.AutoDetectChangesEnabled = false;
        result = base.SavingChangesAsync(eventData, interceptionResult, token);
        context.ChangeTracker.AutoDetectChangesEnabled = true;
        return result;
    }

    protected virtual void BeforeSaving(DbContext context)
    {
        SetShadowProperties(context);
        HandleEvents(context);
    }

    private void SetShadowProperties(DbContext context)
    {
        var service = context.GetService<IIdentityService>();
        context
            .ChangeTracker
            .SetAuditableEntityShadowPropertyValues(service);
    }

    private void HandleEvents(DbContext context)
    {
        var domainEventPipeline = context.GetService<DomainEventPipe>();
        context
            .ChangeTracker
            .AggregateRootsWithEvents()
            .SelectMany(_ => _.Events)
            .ToList()
            .ForEach(async _ => await domainEventPipeline.HandleAsync(_ as dynamic)); // old (dynamic)_
    }

    private DbContext GetContext(DbContextEventData source) => source.Context!;
}

public sealed class EventSourcingSaveChangeInterceptor : SaveInterceptor
{
    protected override void BeforeSaving(DbContext context)
    {
        base.BeforeSaving(context);
        OutboxEvents(context);
    }

    private void OutboxEvents(DbContext context)
    {
        var aggregates = context.ChangeTracker.AggregateRootsWithEvents();
        var userService = context.GetService<IIdentityService>();
        var serializer = context.GetService<ISerializeService>();
        var userId = userService.Id();
        var accuredOn = DateTime.UtcNow;

        var traceId = String.Empty;
        var spanId = String.Empty;
        var activity = Activity.Current;
        if (activity is { })
        {
            traceId = activity.TraceId.ToHexString();
            spanId = activity.SpanId.ToHexString();
        }

        if (aggregates?.Count > 0)
        {
            aggregates.ForEach(_ =>
            {
                var aggregateType = _.Type();
                var events = _.Events;
                var code = events.ElementAt(0).Code.ToString();
                foreach (var _event in events)
                {
                    var eventType = _event.Type();
                    context
                        .Add(new OutboxEvent
                        {
                            State = _event.Mode,
                            UserId = userId,
                            OwnerAggregate = aggregateType.Name,
                            OwnerAggregateType = aggregateType.FullName!,
                            EventName = eventType.Name,
                            EventTypeName = eventType.FullName!,
                            OccurredOnUtc = accuredOn,
                            Payload = serializer.Serialize(_event),
                            TraceId = traceId,
                            SpanId = spanId,
                            IsProccessd = false
                        });
                }
                _.ClearEvents();
            });
        }
    }
}

