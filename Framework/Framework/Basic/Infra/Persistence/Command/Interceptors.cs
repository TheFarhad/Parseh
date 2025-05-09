namespace Framework;

using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class SaveInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> interceptionResult, CancellationToken token = default!)
    {
        var context = GetContext(eventData);

        // از آنجا که متود دیتکت چنجز، به صورت اتوماتیک فقط در متود سیو چینجز فراخوانی می شود
        // ولی ما در اینجا می خواهیم که شدوپراپرتی ها را مقدار دهی کنیم
        // پس نیاز داریم که قبل از سیوچیجز، تغییرات اعمال شوند
        // پس اینکار را خودمان به صورت دستی انجام می دهیم
        context.ChangeTracker.DetectChanges();

        BeforeSaving(context);

        // برای اینکه در متود سیو چینجز، دیتکت چنجز دوباره فراخوانی نشود    
        // و در نتیجه باعث کاهش کارایی نشود
        // دیتکت چنجز را موقتا غیرفعال می کنیم
        // این پراپرتی باعث می شود که در صورت ذخیره، فقط تغییرات در دیتابیس اعمال شوند و دیگر چنج ترکر وارد عمل نشود
        context.ChangeTracker.AutoDetectChangesEnabled = false;

        var result = base.SavingChangesAsync(eventData, interceptionResult, token);

        // در اینجا دوباره دیتکت چنجز را مجددا فعال می کنیم
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

