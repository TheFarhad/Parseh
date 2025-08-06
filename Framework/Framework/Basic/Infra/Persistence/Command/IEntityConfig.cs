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
    /// <param name="b"></param>
    public abstract void ConfigureProperties(EntityTypeBuilder<TEntity> b);

    protected virtual void OnInitialized(EntityTypeBuilder<TEntity> b)
    {
        // آیا درست کار میکند
        //b.ToTable(nameof(TEntity) + "s");

        #region id

        b
           .HasKey(e => e.Id);

        b
           .Property(_ => _.Id)
           .ValueGeneratedOnAdd()
           .Metadata
           .SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);

        #endregion

        #region code

        b
            .HasIndex(_ => _.Code)
            .IsUnique();

        b
            .HasAlternateKey(_ => _.Code);

        b
            .Property(_ => _.Code)
            .HasConversion<CodeConverter>()
            .IsRequired();

        #endregion

        ConfigureProperties(b);

        #region shadow properties

        b
            .Property<string>("CreatedByUserId")
            .IsUnicode(false)
            .HasMaxLength(35); // 36 for guid

        b.Property<DateTime?>("CreatedAt");

        b
            .Property<string>("UpdatedByUserId")
            .IsUnicode(false)
            .HasMaxLength(35); // 36 for guid

        b.Property<DateTime?>("UpdatedAt");

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

