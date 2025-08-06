namespace Parseh.Server.Infra.Persistence.EF.Command;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Domain.Aggregates.User.Entity;
using Core.Domain.Aggregates.Role.Entity;
using Core.Domain.Aggregates.User.ValueObject;
using Core.Domain.Aggregates.Role.ValueObject;

public sealed class UserConfig : AggregateRootConfiguration<User, UserId>
{
    public override void ConfigureProperties(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");

        b.Property(_ => _.Id)
         .HasConversion<UserIdConverter>();

        b
         .Property(_ => _.Name)
         .HasMaxLength(70)
         .IsRequired()
         .IsUnicode(true); // nvarchar(70)

        b
        .Property(_ => _.Family)
        .HasMaxLength(100)
        .IsRequired()
        .IsUnicode(true); // nvarchar(100)

        b
        .Property(_ => _.UserName)
        .HasMaxLength(50)
        .IsRequired()
        .IsUnicode(true); // nvarchar(50)

        b
         .HasIndex(_ => _.UserName)
         .IsUnique(true); // unique non-clusterd index, تکراری نمی تواند باشد

        b
        .Property(_ => _.Password)
        .HasMaxLength(500)
        .IsRequired()
        .IsUnicode(false);

        b
        .Property(_ => _.Salt)
        .HasMaxLength(200)
        .IsRequired()
        .IsUnicode(false);

        #region Navigation Properties

        // -- [ REFRESH TOKEN ] -- \\
        b.HasMany(_ => _.RefreshTokens)
         .WithOne()
         .HasForeignKey("RefreshTokenId")
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);
        // TODO: هر چند که ما رکوردی را حذف نمیکنیم
        // و فقط دیلیتد آن را ترو میکنیم

        b
        .Metadata
        .FindNavigation(nameof(User.RefreshTokens))?
        .SetPropertyAccessMode(PropertyAccessMode.Field);

        b
        .Navigation(_ => _.RefreshTokens)
        .Metadata
        .SetField(User.RefreshTokenBackingField);


        // -- [ ROLES ] -- \\
        b
         .HasMany(_ => _.Roles)
         .WithOne(_ => _.User)
         .HasForeignKey(_ => _.UserId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);

        b
        .Metadata
        .FindNavigation(nameof(User.Roles))?
        .SetPropertyAccessMode(PropertyAccessMode.Field);

        b
        .Navigation(_ => _.Roles)
        .Metadata
        .SetField(User.RoleBackingField);

        #endregion
    }
}

public sealed class RoleConfig : AggregateRootConfiguration<Role, RoleId>
{
    public override void ConfigureProperties(EntityTypeBuilder<Role> b)
    {
        b.ToTable("Roles");

        b.Property(_ => _.Id)
         .HasConversion<RoleIdConverter>();

        b
         .Property(_ => _.Title)
         .HasMaxLength(70)
         .IsRequired()
         .IsUnicode(false);
        b
        .Property(_ => _.Display)
        .HasMaxLength(100)
        .IsRequired()
        .IsUnicode(true);

        b
        .Property(_ => _.Description)
        .HasMaxLength(200)
        .IsRequired()
        .IsUnicode(true);

        // -- [ PERMISSIONS ] -- \\
        b
         .HasMany(_ => _.Permissions)
         .WithOne(_ => _.Role)
         .HasForeignKey(_ => _.RoleId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);

        b
        .Metadata
        .FindNavigation(nameof(Role.Permissions))?
        .SetPropertyAccessMode(PropertyAccessMode.Field);

        b
        .Navigation(_ => _.Permissions)
        .Metadata
        .SetField(Role.PermissionsBackingField);
    }
}

public sealed class RefreshTokenConfig : EntityConfiguration<RefreshToken, RefreshTokenId>
{
    public override void ConfigureProperties(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("RefreshTokens");

        b.Property(_ => _.Id)
         .HasConversion<RefreshTokenIdConverter>();

        b
         .Property(_ => _.HashedToken)
         // TODO: بعد از هش شدن، تعداد کارکترهای آن چند می شود؟؟
         .HasMaxLength(100)
         .IsRequired()
         .IsUnicode(false);

        b
         .HasIndex(_ => _.HashedToken)
         .IsUnique();

        //b
        // .Property(_ => _.UserCode)
        // .HasMaxLength(36)
        // .IsRequired()
        // .IsUnicode(false);

        b
         .Property(_ => _.RemoteIp)
         .HasMaxLength(15)
         .IsRequired()
         .IsUnicode(false);

        b
        .Property(_ => _.UserAgent)
        .HasMaxLength(256)
        .IsRequired()
        .IsUnicode(false);

        b
         .Property(_ => _.CreateAt)
         .HasColumnType("datetime2")
         .HasDefaultValueSql("GETUTCDATE()");

        b
         .Property(_ => _.IsRevoked);

        b
        .Property(_ => _.RevokedAt)
        .HasColumnType("datetime2");

        b
        .Property(_ => _.RevokedByRemoteIp)
        .HasMaxLength(15)
        .IsRequired()
        .IsUnicode(false);

        b
         .Property(_ => _.RevokeReason)
         .HasMaxLength(300)
         .IsRequired()
         .IsUnicode(true);

        b
        .Property(_ => _.ReplacedByToken)
        // TODO: بعد از هش شدن، تعداد کارکترهای آن چند می شود؟؟
        .HasMaxLength(100)
        .IsRequired()
        .IsUnicode(false);

        b
        .Property(_ => _.ExpireAt)
        .HasColumnType("datetime2");
    }
}

public sealed class PermissionConfig : EntityConfiguration<Permission, PermissionId>
{
    public override void ConfigureProperties(EntityTypeBuilder<Permission> b)
    {
        b.ToTable("Permissions");

        b.Property(_ => _.Id)
         .HasConversion<PermissionIdConverter>();

        b
         .Property(_ => _.Title)
         .HasMaxLength(70)
         .IsRequired()
         .IsUnicode(false);
        b
        .Property(_ => _.Display)
        .HasMaxLength(100)
        .IsRequired()
        .IsUnicode(true);

        b
        .Property(_ => _.Description)
        .HasMaxLength(200)
        .IsRequired()
        .IsUnicode(true);
    }
}

public sealed class UserRoleConfig : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> b)
    {
        b
         .ToTable("UserRoles");

        // TODO:
        //      اگر این جدول نوع انتیتی بود و آیدی داشت
        //      برای اینکه برای آیدی، کلاستر ایندکس ایجاد نشود
        //      باید به صورت زیر عمل کرد

        //b.HasKey(_ => _.Id).IsClustered(false);
        // OR ...
        //b
        // .Property(x => x.Id)
        // .IsRequired()
        // .UseIdentityColumn();
        //b
        // .HasIndex(x => x.Id)
        // .IsUnique()
        // .IsClustered(false);

        b.Property(_ => _.UserId)
         .HasConversion<UserIdConverter>();

        b.Property(_ => _.RoleId)
         .HasConversion<RoleIdConverter>();

        b
         .HasKey(_ => new { _.UserId, _.RoleId })
         .IsClustered(true);

        #region Navigation Properties

        b
         .HasOne(_ => _.User)
         .WithMany(_ => _.Roles)
         .HasForeignKey(_ => _.UserId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);

        b
        .HasOne(_ => _.Role)
        .WithMany()
        .HasForeignKey(_ => _.RoleId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);


        #endregion
    }
}

public sealed class RolePermissionConfig : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> b)
    {
        b
        .ToTable("RolePermissions");

        b.Property(_ => _.RoleId)
         .HasConversion<RoleIdConverter>();

        b.Property(_ => _.PermissionId)
         .HasConversion<PermissionIdConverter>();

        b
         .HasKey(_ => new { _.RoleId, _.PermissionId })
         .IsClustered(true);

        // TODO:
        //      اگر این جدول نوع انتیتی بود و آیدی داشت
        //      برای اینکه برای آیدی، کلاستر ایندکس ایجاد نشود
        //      باید به صورت زیر عمل کرد

        //b.HasKey(_ => _.Id).IsClustered(false);
        // OR ...
        //b
        // .Property(_ => _.Id)
        // .IsRequired()
        // .UseIdentityColumn();
        //b
        // .HasIndex(_ => _.Id)
        // .IsUnique()
        // .IsClustered(false);


        #region Navigation Properties

        b
         .HasOne(_ => _.Role)
         .WithMany(_ => _.Permissions)
         .HasForeignKey(_ => _.RoleId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);

        b
        .HasOne(_ => _.Permission)
        .WithMany()
        .HasForeignKey(_ => _.PermissionId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

        #endregion
    }
}



