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
         .IsUnique(true);

        b
        .Property(_ => _.Password)
        .HasMaxLength(500)
        .IsRequired()
        .IsUnicode(false);

        //b
        //.Property(_ => _.Salt)
        //.HasMaxLength(200)
        //.IsRequired()
        //.IsUnicode(false);

        #region Navigation Properties

        // -- [ REFRESH TOKEN ] -- \\
        b.HasMany<RefreshToken>(_ => _.RefreshTokens)
         .WithOne()
         .HasForeignKey("UserId")
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
         .HasMany<UserRole>(_ => _.Roles)
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
         .HasMany<RoleClaim>(_ => _.Cliams)
         .WithOne(_ => _.Role)
         .HasForeignKey(_ => _.RoleId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);

        b
        .Metadata
        .FindNavigation(nameof(Role.Cliams))?
        .SetPropertyAccessMode(PropertyAccessMode.Field);

        b
        .Navigation(_ => _.Cliams)
        .Metadata
        .SetField(Role.ClaimsBackingField);
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
        .IsRequired(false)
        .IsUnicode(false);

        b
         .Property(_ => _.RevokeReason)
         .HasMaxLength(300)
         .IsRequired(false)
         .IsUnicode(true);

        b
        .Property(_ => _.ReplacedByToken)
        // TODO: بعد از هش شدن، تعداد کارکترهای آن چند می شود؟؟
        .HasMaxLength(100)
        .IsRequired(false)
        .IsUnicode(false);

        b
        .Property(_ => _.ExpireAt)
        .HasColumnType("datetime2");
    }
}

public sealed class ClaimConfig : EntityConfiguration<Claim, ClaimId>
{
    public override void ConfigureProperties(EntityTypeBuilder<Claim> b)
    {
        b.ToTable("Claims");

        b.Property(_ => _.Id)
         .HasConversion<ClaimIdConverter>();

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
        /*
           اگر انتیتی بود و آیدی داشت
           برای اینکه کلاستر ایندکس ایجاد نشود
           باید به صورت زیر عمل کرد

           b
            .HasKey(e => e.Id)
            .IsClustered(false);
           b
            .HasIndex(e => new { e.UserId, e.RoleId })
            .IsUnique()
            .IsClustered(true);

            در نتیجه یک پرایمری کی بدون ایندکس داریم
            و یک یونیک کلاسترد ایندکس کامپوزیت
         */

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

public sealed class RoleClaimConfig : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> b)
    {
        b
        .ToTable("RoleClaims");

        b.Property(_ => _.RoleId)
         .HasConversion<RoleIdConverter>();

        b.Property(_ => _.ClaimId)
         .HasConversion<ClaimIdConverter>();

        b
         .HasKey(_ => new { _.RoleId, _.ClaimId })
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
         .WithMany(_ => _.Cliams)
         .HasForeignKey(_ => _.RoleId)
         .IsRequired()
         .OnDelete(DeleteBehavior.Cascade);

        b
        .HasOne(_ => _.Cliam)
        .WithMany()
        .HasForeignKey(_ => _.ClaimId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

        #endregion
    }
}



