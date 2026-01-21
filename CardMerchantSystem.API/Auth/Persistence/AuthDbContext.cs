using CardMerchantSystem.API.Auth.Entities;
using CardMerchantSystem.API.Auth.Enums;
using CardMerchantSystem.API.Auth.Constants;
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
    public DbSet<MenuEntity> Menus => Set<MenuEntity>();
    public DbSet<MenuClaimEntity> MenuClaims => Set<MenuClaimEntity>();

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

        // Menu Configuration
        modelBuilder.Entity<MenuEntity>(entity =>
        {
            entity.ToTable("AuthMenus");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Icon).HasMaxLength(50);
            entity.Property(e => e.Path).HasMaxLength(200);
            entity.HasIndex(e => e.Name).IsUnique();

            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // MenuClaim Configuration
        modelBuilder.Entity<MenuClaimEntity>(entity =>
        {
            entity.ToTable("AuthMenuClaims");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ClaimType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.ClaimValue).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.Menu)
                .WithMany(m => m.Claims)
                .HasForeignKey(e => e.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.MenuId, e.ClaimType, e.ClaimValue }).IsUnique();
        });

        // Seed Data
        SeedRoles(modelBuilder);
        SeedAdminUser(modelBuilder);
        SeedMenus(modelBuilder);
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

        modelBuilder.Entity<UserRoleEntity>().HasData(new UserRoleEntity
        {
            UserId = adminId,
            RoleId = (int)SystemRole.Admin,
            AssignedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            AssignedBy = "System"
        });
    }

    private static void SeedMenus(ModelBuilder modelBuilder)
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Parent Menu IDs
        var dashboardId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var cardParentId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var merchantParentId = Guid.Parse("10000000-0000-0000-0000-000000000003");
        var financeParentId = Guid.Parse("10000000-0000-0000-0000-000000000004");
        var workOrderId = Guid.Parse("10000000-0000-0000-0000-000000000005");
        var systemParentId = Guid.Parse("10000000-0000-0000-0000-000000000006");

        // Child Menu IDs
        var cardsId = Guid.Parse("20000000-0000-0000-0000-000000000001");
        var cardApplicationsId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var cardBlocksId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var merchantsId = Guid.Parse("20000000-0000-0000-0000-000000000004");
        var terminalsId = Guid.Parse("20000000-0000-0000-0000-000000000005");
        var transactionsId = Guid.Parse("20000000-0000-0000-0000-000000000006");
        var statementsId = Guid.Parse("20000000-0000-0000-0000-000000000007");
        var accountingId = Guid.Parse("20000000-0000-0000-0000-000000000008");
        var usersId = Guid.Parse("20000000-0000-0000-0000-000000000009");
        var rolesId = Guid.Parse("20000000-0000-0000-0000-000000000010");
        var menusId = Guid.Parse("20000000-0000-0000-0000-000000000011");

        // Menus
        var menus = new List<MenuEntity>
        {
            // Dashboard
            new() { Id = dashboardId, ParentId = null, Name = "dashboard", Title = "Dashboard", Icon = "layout-dashboard", Path = "/dashboard", DisplayOrder = 1, IsActive = true, IsVisible = true, CreatedAt = createdAt },

            // Kart Yönetimi (Parent)
            new() { Id = cardParentId, ParentId = null, Name = "card-management", Title = "Kart Yönetimi", Icon = "credit-card", Path = null, DisplayOrder = 2, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = cardsId, ParentId = cardParentId, Name = "cards", Title = "Kartlar", Icon = "credit-card", Path = "/cards", DisplayOrder = 1, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = cardApplicationsId, ParentId = cardParentId, Name = "card-applications", Title = "Başvurular", Icon = "file-text", Path = "/card-applications", DisplayOrder = 2, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = cardBlocksId, ParentId = cardParentId, Name = "card-blocks", Title = "Blokeler", Icon = "shield-off", Path = "/card-blocks", DisplayOrder = 3, IsActive = true, IsVisible = true, CreatedAt = createdAt },

            // Üye İşyeri (Parent)
            new() { Id = merchantParentId, ParentId = null, Name = "merchant-management", Title = "Üye İşyeri", Icon = "store", Path = null, DisplayOrder = 3, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = merchantsId, ParentId = merchantParentId, Name = "merchants", Title = "İşyerleri", Icon = "building", Path = "/merchants", DisplayOrder = 1, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = terminalsId, ParentId = merchantParentId, Name = "terminals", Title = "Terminaller", Icon = "monitor", Path = "/terminals", DisplayOrder = 2, IsActive = true, IsVisible = true, CreatedAt = createdAt },

            // Finans (Parent)
            new() { Id = financeParentId, ParentId = null, Name = "finance-management", Title = "Finans", Icon = "wallet", Path = null, DisplayOrder = 4, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = transactionsId, ParentId = financeParentId, Name = "transactions", Title = "İşlemler", Icon = "arrow-left-right", Path = "/transactions", DisplayOrder = 1, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = statementsId, ParentId = financeParentId, Name = "statements", Title = "Ekstreler", Icon = "file-spreadsheet", Path = "/statements", DisplayOrder = 2, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = accountingId, ParentId = financeParentId, Name = "accounting", Title = "Muhasebe", Icon = "calculator", Path = "/accounting", DisplayOrder = 3, IsActive = true, IsVisible = true, CreatedAt = createdAt },

            // İş Emirleri
            new() { Id = workOrderId, ParentId = null, Name = "work-orders", Title = "İş Emirleri", Icon = "clipboard-list", Path = "/work-orders", DisplayOrder = 5, IsActive = true, IsVisible = true, CreatedAt = createdAt },

            // Sistem (Parent)
            new() { Id = systemParentId, ParentId = null, Name = "system-management", Title = "Sistem", Icon = "settings", Path = null, DisplayOrder = 6, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = usersId, ParentId = systemParentId, Name = "users", Title = "Kullanıcılar", Icon = "users", Path = "/users", DisplayOrder = 1, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = rolesId, ParentId = systemParentId, Name = "roles", Title = "Roller", Icon = "shield", Path = "/roles", DisplayOrder = 2, IsActive = true, IsVisible = true, CreatedAt = createdAt },
            new() { Id = menusId, ParentId = systemParentId, Name = "menus", Title = "Menüler", Icon = "menu", Path = "/menus", DisplayOrder = 3, IsActive = true, IsVisible = true, CreatedAt = createdAt },
        };

        modelBuilder.Entity<MenuEntity>().HasData(menus);

        // Menu Claims
        var claims = new List<MenuClaimEntity>
        {
            // Dashboard - Herkes görebilir
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), MenuId = dashboardId, ClaimType = "Policy", ClaimValue = Policies.ViewerOrAbove },

            // Kart Yönetimi
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), MenuId = cardParentId, ClaimType = "Policy", ClaimValue = Policies.CardManagement },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), MenuId = cardsId, ClaimType = "Policy", ClaimValue = Policies.CardManagement },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), MenuId = cardApplicationsId, ClaimType = "Policy", ClaimValue = Policies.CardManagement },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000005"), MenuId = cardBlocksId, ClaimType = "Policy", ClaimValue = Policies.CardManagement },

            // Üye İşyeri
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000006"), MenuId = merchantParentId, ClaimType = "Policy", ClaimValue = Policies.MerchantManagement },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000007"), MenuId = merchantsId, ClaimType = "Policy", ClaimValue = Policies.MerchantManagement },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000008"), MenuId = terminalsId, ClaimType = "Policy", ClaimValue = Policies.MerchantManagement },

            // Finans
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000009"), MenuId = financeParentId, ClaimType = "Policy", ClaimValue = Policies.FinanceManagement },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000010"), MenuId = transactionsId, ClaimType = "Policy", ClaimValue = Policies.FinanceManagement },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000011"), MenuId = statementsId, ClaimType = "Policy", ClaimValue = Policies.FinanceManagement },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000012"), MenuId = accountingId, ClaimType = "Policy", ClaimValue = Policies.FinanceManagement },

            // İş Emirleri
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000013"), MenuId = workOrderId, ClaimType = "Policy", ClaimValue = Policies.WorkOrderManagement },

            // Sistem - Sadece Admin
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000014"), MenuId = systemParentId, ClaimType = "Policy", ClaimValue = Policies.AdminOnly },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000015"), MenuId = usersId, ClaimType = "Policy", ClaimValue = Policies.AdminOnly },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000016"), MenuId = rolesId, ClaimType = "Policy", ClaimValue = Policies.AdminOnly },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000017"), MenuId = menusId, ClaimType = "Policy", ClaimValue = Policies.AdminOnly },
        };

        modelBuilder.Entity<MenuClaimEntity>().HasData(claims);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}