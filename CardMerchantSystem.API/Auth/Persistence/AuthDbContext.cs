using CardMerchantSystem.API.Auth.Entities;
using CardMerchantSystem.API.Auth.Enums;
using DocumentFormat.OpenXml.Math;
using Microsoft.EntityFrameworkCore;

namespace CardMerchantSystem.API.Auth.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("AuthUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Role Configuration
        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.ToTable("AuthRoles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // UserRole Configuration
        modelBuilder.Entity<UserRoleEntity>(entity =>
        {
            entity.ToTable("AuthUserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.AssignedBy).HasMaxLength(100);
        });

        // Seed Roles
        SeedRoles(modelBuilder);

        // Seed Default Admin User
        SeedAdminUser(modelBuilder);
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        var roles = Enum.GetValues<SystemRole>()
            .Select(RoleEntity.FromEnum)
            .ToArray();

        modelBuilder.Entity<RoleEntity>().HasData(roles);
    }

    private static void SeedAdminUser(ModelBuilder modelBuilder)
    {
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Admin kullanıcı
        modelBuilder.Entity<UserEntity>().HasData(new UserEntity
        {
            Id = adminId,
            Username = "admin",
            Email = "admin@cardmerchant.com",
            PasswordHash = HashPassword("Admin123!"),
            FullName = "Sistem Yöneticisi",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Admin rolü ata
        modelBuilder.Entity<UserRoleEntity>().HasData(new UserRoleEntity
        {
            UserId = adminId,
            RoleId = (int)SystemRole.Admin,
            AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            AssignedBy = "System"
        });
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}