namespace Framework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public interface IEntityConfig<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TId : Identity
    where TEntity : Entity<TId>
{ }

public abstract class EntityConfiguration<TEntity, TId> : IEntityConfig<TEntity, TId>
    where TId : Identity
    where TEntity : Entity<TId>
{
    public void Configure(EntityTypeBuilder<TEntity> builder) => OnInitialized(builder);

    /// <summary>
    /// Ignore Id, Code And ShadowProperties
    /// </summary>
    /// <param name="builder"></param>
    public abstract void ConfigureProperties(EntityTypeBuilder<TEntity> builder);

    protected virtual void OnInitialized(EntityTypeBuilder<TEntity> builder)
    {
        #region id

        builder
           .HasKey(e => e.Id);

        builder
           .Property(_ => _.Id)
           .ValueGeneratedOnAdd()
           .Metadata
           .SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        #endregion

        #region code

        builder
            .HasIndex(_ => _.Code)
            .IsUnique();

        builder
            .HasAlternateKey(_ => _.Code);

        builder
            .Property(_ => _.Code)
            .HasConversion<CodeConverter>()
            .IsRequired();

        #endregion

        ConfigureProperties(builder);

        #region shadow properties

        builder
            .Property<string>("CreatedByUserId")
            .IsUnicode(false)
            .HasMaxLength(35); // 36 for guid

        builder.Property<DateTime?>("CreatedAt");

        builder
            .Property<string>("UpdatedByUserId")
            .IsUnicode(false)
            .HasMaxLength(35); // 36 for guid

        builder.Property<DateTime?>("UpdatedAt");

        #endregion
    }
}

public abstract class AggregateRootConfiguration<TAggregateRoot, TId> : EntityConfiguration<TAggregateRoot, TId>
    where TId : Identity
    where TAggregateRoot : AggregateRoot<TId>
{
    protected override void OnInitialized(EntityTypeBuilder<TAggregateRoot> builder)
    {
        // icnlude id, code, other properties, shadow properties
        base.OnInitialized(builder);

        #region ignore version

        builder
            .Ignore(_ => _.Version);

        #endregion
    }
}

