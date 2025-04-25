namespace Framework;

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class ChangeTrackerExtensions
{
    public static void SetAuditableEntityShadowPropertyValues(this ChangeTracker source, IIdentityService userService)
    {
        var entities = source.Entries<IEntity>();
        var userId = userService.Id();
        var dateTime = DateTime.UtcNow;

        var createdEntries = entities.Where(_ => _.State is EntityState.Added);
        foreach (var _ in createdEntries)
        {
            _.Property<string>(ModelBuilderExtensions.CreatedByUserId).CurrentValue = userId;
            _.Property<DateTime?>(ModelBuilderExtensions.CreatedAt).CurrentValue = dateTime;
        }

        var updatedEntries = entities.Where(_ => _.State is EntityState.Modified);
        foreach (var _ in updatedEntries)
        {
            _.Property<string>(ModelBuilderExtensions.UpdatedByUserId).CurrentValue = userId;
            _.Property<DateTime?>(ModelBuilderExtensions.UpdatedAt).CurrentValue = dateTime;
        }
    }

    public static List<IAggregateRoot> AggregateRootsWithEvents(this ChangeTracker source)
        => source
            .TrackedEntities(HasEvent)
            .Select(_ => _.Entity)
            .ToList();

    public static List<EntityEntry<IAggregateRoot>> ModifiedAggregatRoots(this ChangeTracker source)
        => source.TrackedEntities(IsUpdated);

    private static List<EntityEntry<IAggregateRoot>> TrackedEntities(this ChangeTracker source, Func<EntityEntry<IAggregateRoot>, bool> predicate = null!)
    {
        var result = source.Entries<IAggregateRoot>();
        if (predicate is { })
        {
            result = result.Where(predicate);
        }
        return result.ToList();
    }

    private static Func<EntityEntry<IAggregateRoot>, bool> IsUpdated
        => (_) => _.State is EntityState.Modified;

    private static Func<EntityEntry<IAggregateRoot>, bool> IsAdded
        => (_) => _.State is EntityState.Added;

    private static Func<EntityEntry<IAggregateRoot>, bool> IsDeleted
        => (_) => _.State is EntityState.Deleted;

    private static Func<EntityEntry<IAggregateRoot>, bool> HasEvent
        => (_) => _.Entity.Events.Count > 0;
}

public static class ModelBuilderExtensions
{
    public static readonly string CreatedByUserId = nameof(CreatedByUserId);
    public static readonly Func<object, string> EFPropertyCreatedByUserId =
        _ => EFProperty<string>(_, CreatedByUserId);

    public static readonly string UpdatedByUserId = nameof(UpdatedByUserId);
    public static readonly Func<object, string> EFPropertyModifiedByUserId =
        _ => EFProperty<string>(_, UpdatedByUserId);

    public static readonly string CreatedAt = nameof(CreatedAt);
    public static readonly Func<object, DateTime?> EFPropertyCreatedAt =
        _ => EFProperty<DateTime?>(_, CreatedAt);

    public static readonly string UpdatedAt = nameof(UpdatedAt);
    public static readonly Func<object, DateTime?> EFPropertyModifiedAt =
        _ => EFProperty<DateTime?>(_, UpdatedAt);

    public static ModelBuilder InitializeEntities(this ModelBuilder source)
    {
        source
          .EntitiesTypes<IEntity>()
          .ToList()
          .ForEach(_ =>
          {
              source
               .Entity(_.ClrType)
               .ConfigId()
               .ConfigCode()
               .AddShadowProperties();
              //.NonClusterdIndexOnDeletedProp();
          });

        return source;
    }

    public static ModelBuilder IgnoreVersionInAggregateRoots(this ModelBuilder source)
    {
        // به کلاس زیر انتقال داده شده و دیگر نیازی نیست اینجا نوشته شود
        // البته اگر خطایی وجود نداشته باشد
        //EntityConfiguration

        //source
        //  .EntitiesTypes<IAggregateRoot>()
        //  .ToList()
        //  .ForEach(_ =>
        //  {
        //      source
        //      .Entity(_.ClrType)
        //      .Ignore("Version");
        //  });
        return source;
    }

    public static IEnumerable<IMutableEntityType> EntitiesTypes<TEntity>(this ModelBuilder source)
       where TEntity : IEntity
       => source
           .EfEntitiesTypes(_ => typeof(TEntity).IsAssignableFrom(_.ClrType));

    public static ModelBuilder SoftDeletableQueryFilter(this ModelBuilder source)
    {
        Expression<Func<SoftDeletableModel, bool>> func = _ => !_.Deleted;
        foreach (var entityType in source.EfEntitiesTypes(_ => typeof(SoftDeletableModel).IsAssignableFrom(_.ClrType)))
        {
            var parameterExpression = Expression.Parameter(entityType.ClrType);
            var body = ReplacingExpressionVisitor.Replace(func.Parameters.First(), parameterExpression, func.Body);

            entityType
                .SetQueryFilter(Expression.Lambda(body, parameterExpression));
        }
        return source;
    }

    private static List<IMutableEntityType> EfEntitiesTypes(this ModelBuilder source, Func<IMutableEntityType, bool> predicate = null!)
    {
        var result = source.Model.GetEntityTypes();
        if (predicate is not null)
        {
            result = result.Where(predicate);
        }
        return result.ToList();
    }

    private static EntityTypeBuilder NonClusterdIndexOnDeletedProp(this EntityTypeBuilder source)
    {
        /*
         ایندکس کردن ستون دیلیتد کار بسیار غلطی است
        هیچ مزیتی ایجاد نمیکند و حتی پرفورمنس دیتابیس را هم کم میکند
        می توان آن را به صورت ترکیبی با ستونهای مورد نیاز دیگر، به صورت کامپوزیت نانکلاسترد ایندکس استفاده کرد
         */

        //if (source.GetType().IsAssignableFrom(typeof(SoftDeletableSource)))
        //{
        //    var deleted = nameof(SoftDeletableSource.Deleted);
        //    source
        //        .HasIndex(deleted)
        //        .HasFilter($"{deleted} = 0");
        //}

        return source;
    }

    private static EntityTypeBuilder ConfigId(this EntityTypeBuilder entityTypeBuilder)
    {
        // به کلاس زیر انتقال داده شده و دیگر نیازی نیست اینجا نوشته شود
        // البته اگر خطایی وجود نداشته باشد
        //EntityConfiguration

        //const string id = "Id";
        //entityTypeBuilder.HasKey(id);
        return entityTypeBuilder;
    }

    private static EntityTypeBuilder ConfigCode(this EntityTypeBuilder entityTypeBuilder)
    {
        // Entity Name = entityTypeBuilder.Metadata.ClrType.Name
        // برای نام گذاری شخصی کلید یونیک مفید است


        // به کلاس زیر انتقال داده شده و دیگر نیازی نیست اینجا نوشته شود
        // البته اگر خطایی وجود نداشته باشد
        //EntityConfiguration

        //var code = nameof(Code);

        //entityTypeBuilder
        //    .Property<Code>(code)
        //    .IsRequired();

        //entityTypeBuilder
        //    .HasIndex(code)
        //    .IsUnique();

        //entityTypeBuilder
        //    .HasAlternateKey(code);

        return entityTypeBuilder;
    }

    private static EntityTypeBuilder AddShadowProperties(this EntityTypeBuilder entityTypeBuilder)
    {
        // به کلاس زیر انتقال داده شده و دیگر نیازی نیست اینجا نوشته شود
        // البته اگر خطایی وجود نداشته باشد
        //EntityConfiguration

        //entityTypeBuilder
        //    .Property<string>(CreatedByUserId)
        //    .IsUnicode(false)
        //    .HasMaxLength(35);

        //entityTypeBuilder
        //    .Property<DateTime?>(CreatedAt);

        //entityTypeBuilder
        //    .Property<string>(UpdatedByUserId)
        //    .IsUnicode(false)
        //    .HasMaxLength(35);

        //entityTypeBuilder
        //    .Property<DateTime?>(UpdatedAt);

        return entityTypeBuilder;
    }

    private static TType EFProperty<TType>(object source, string property)
        => EF.Property<TType>(source, property);

    //public static ModelBuilder UseValueConverterForType<T>(this ModelBuilder source, ValueConverter converter, int maxLength = 0)
    //{
    //    foreach (var item in source.EntityTypes())
    //    {
    //        var properties = item
    //            .ClrType
    //            .GetProperties()
    //            .Where(p => p.PropertyType == typeof(T));

    //        foreach (var property in properties)
    //        {
    //            var entity = source
    //                .Entity(item.Name)
    //                .Property(property.Name);

    //            entity.HasConversion(converter);

    //            if (maxLength > 0) entity.HasMaxLength(maxLength);
    //        }
    //    }
    //    return source;
}

public static class RowVersionShadowPropertyExtensions
{
    public static readonly string RowVersion = nameof(RowVersion);

    public static void AddRowVersionShadowProperty<TEntity>(this EntityTypeBuilder<TEntity> source) where TEntity : class
        => source.Property<byte[]>(RowVersion).IsRowVersion();
}