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
    public DbSet<LanguageEntity> Languages => Set<LanguageEntity>();
    public DbSet<TranslationEntity> Translations => Set<TranslationEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Language Configuration
        modelBuilder.Entity<LanguageEntity>(entity =>
        {
            entity.ToTable("Languages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(5);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NativeName).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Translation Configuration
        modelBuilder.Entity<TranslationEntity>(entity =>
        {
            entity.ToTable("Translations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LanguageCode).IsRequired().HasMaxLength(5);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => new { e.LanguageCode, e.Key }).IsUnique();
            entity.HasIndex(e => e.Category);
        });

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
        SeedLanguages(modelBuilder);
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

    private static void SeedLanguages(ModelBuilder modelBuilder)
    {
        var createdAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Diller
        var languages = new List<LanguageEntity>
    {
        new() { Id = 1, Code = "tr", Name = "Turkish", NativeName = "Türkçe", IsActive = true, IsDefault = true, CreatedAt = createdAt },
        new() { Id = 2, Code = "en", Name = "English", NativeName = "English", IsActive = true, IsDefault = false, CreatedAt = createdAt }
    };

        modelBuilder.Entity<LanguageEntity>().HasData(languages);

        // Çeviriler
        var translations = new List<TranslationEntity>
    {
        // === MENU - Turkish ===
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000001"), LanguageCode = "tr", Key = "menu.dashboard", Value = "Dashboard", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000002"), LanguageCode = "tr", Key = "menu.card-management", Value = "Kart Yönetimi", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000003"), LanguageCode = "tr", Key = "menu.cards", Value = "Kartlar", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000004"), LanguageCode = "tr", Key = "menu.card-applications", Value = "Başvurular", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000005"), LanguageCode = "tr", Key = "menu.card-blocks", Value = "Blokeler", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000006"), LanguageCode = "tr", Key = "menu.merchant-management", Value = "Üye İşyeri", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000007"), LanguageCode = "tr", Key = "menu.merchants", Value = "İşyerleri", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000008"), LanguageCode = "tr", Key = "menu.terminals", Value = "Terminaller", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000009"), LanguageCode = "tr", Key = "menu.finance-management", Value = "Finans", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000010"), LanguageCode = "tr", Key = "menu.transactions", Value = "İşlemler", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000011"), LanguageCode = "tr", Key = "menu.statements", Value = "Ekstreler", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000012"), LanguageCode = "tr", Key = "menu.accounting", Value = "Muhasebe", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000013"), LanguageCode = "tr", Key = "menu.work-orders", Value = "İş Emirleri", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000014"), LanguageCode = "tr", Key = "menu.system-management", Value = "Sistem", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000015"), LanguageCode = "tr", Key = "menu.users", Value = "Kullanıcılar", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000016"), LanguageCode = "tr", Key = "menu.roles", Value = "Roller", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0001-000000000017"), LanguageCode = "tr", Key = "menu.menus", Value = "Menüler", Category = "menu", CreatedAt = createdAt },

        // === MENU - English ===
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000001"), LanguageCode = "en", Key = "menu.dashboard", Value = "Dashboard", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000002"), LanguageCode = "en", Key = "menu.card-management", Value = "Card Management", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000003"), LanguageCode = "en", Key = "menu.cards", Value = "Cards", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000004"), LanguageCode = "en", Key = "menu.card-applications", Value = "Applications", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000005"), LanguageCode = "en", Key = "menu.card-blocks", Value = "Blocks", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000006"), LanguageCode = "en", Key = "menu.merchant-management", Value = "Merchant", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000007"), LanguageCode = "en", Key = "menu.merchants", Value = "Merchants", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000008"), LanguageCode = "en", Key = "menu.terminals", Value = "Terminals", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000009"), LanguageCode = "en", Key = "menu.finance-management", Value = "Finance", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000010"), LanguageCode = "en", Key = "menu.transactions", Value = "Transactions", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000011"), LanguageCode = "en", Key = "menu.statements", Value = "Statements", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000012"), LanguageCode = "en", Key = "menu.accounting", Value = "Accounting", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000013"), LanguageCode = "en", Key = "menu.work-orders", Value = "Work Orders", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000014"), LanguageCode = "en", Key = "menu.system-management", Value = "System", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000015"), LanguageCode = "en", Key = "menu.users", Value = "Users", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000016"), LanguageCode = "en", Key = "menu.roles", Value = "Roles", Category = "menu", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0002-000000000017"), LanguageCode = "en", Key = "menu.menus", Value = "Menus", Category = "menu", CreatedAt = createdAt },

        // === ROLES - Turkish ===
        new() { Id = Guid.Parse("40000000-0000-0000-0003-000000000001"), LanguageCode = "tr", Key = "role.Admin", Value = "Sistem Yöneticisi", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0003-000000000002"), LanguageCode = "tr", Key = "role.CardOperator", Value = "Kart Operasyon", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0003-000000000003"), LanguageCode = "tr", Key = "role.MerchantOperator", Value = "Üye İşyeri Operasyon", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0003-000000000004"), LanguageCode = "tr", Key = "role.FinanceOperator", Value = "Finans Operasyon", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0003-000000000005"), LanguageCode = "tr", Key = "role.ComplianceOfficer", Value = "Uyum Sorumlusu", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0003-000000000006"), LanguageCode = "tr", Key = "role.CallCenterAgent", Value = "Çağrı Merkezi", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0003-000000000007"), LanguageCode = "tr", Key = "role.Viewer", Value = "Görüntüleyici", Category = "role", CreatedAt = createdAt },

        // === ROLES - English ===
        new() { Id = Guid.Parse("40000000-0000-0000-0004-000000000001"), LanguageCode = "en", Key = "role.Admin", Value = "System Administrator", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0004-000000000002"), LanguageCode = "en", Key = "role.CardOperator", Value = "Card Operator", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0004-000000000003"), LanguageCode = "en", Key = "role.MerchantOperator", Value = "Merchant Operator", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0004-000000000004"), LanguageCode = "en", Key = "role.FinanceOperator", Value = "Finance Operator", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0004-000000000005"), LanguageCode = "en", Key = "role.ComplianceOfficer", Value = "Compliance Officer", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0004-000000000006"), LanguageCode = "en", Key = "role.CallCenterAgent", Value = "Call Center Agent", Category = "role", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0004-000000000007"), LanguageCode = "en", Key = "role.Viewer", Value = "Viewer", Category = "role", CreatedAt = createdAt },

        // === COMMON - Turkish ===
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000001"), LanguageCode = "tr", Key = "common.save", Value = "Kaydet", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000002"), LanguageCode = "tr", Key = "common.cancel", Value = "İptal", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000003"), LanguageCode = "tr", Key = "common.delete", Value = "Sil", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000004"), LanguageCode = "tr", Key = "common.edit", Value = "Düzenle", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000005"), LanguageCode = "tr", Key = "common.add", Value = "Ekle", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000006"), LanguageCode = "tr", Key = "common.search", Value = "Ara", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000007"), LanguageCode = "tr", Key = "common.filter", Value = "Filtrele", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000008"), LanguageCode = "tr", Key = "common.refresh", Value = "Yenile", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000009"), LanguageCode = "tr", Key = "common.yes", Value = "Evet", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000010"), LanguageCode = "tr", Key = "common.no", Value = "Hayır", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000011"), LanguageCode = "tr", Key = "common.loading", Value = "Yükleniyor...", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000012"), LanguageCode = "tr", Key = "common.noData", Value = "Veri bulunamadı", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000013"), LanguageCode = "tr", Key = "common.success", Value = "Başarılı", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000014"), LanguageCode = "tr", Key = "common.error", Value = "Hata", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000015"), LanguageCode = "tr", Key = "common.warning", Value = "Uyarı", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000016"), LanguageCode = "tr", Key = "common.confirm", Value = "Onayla", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000017"), LanguageCode = "tr", Key = "common.back", Value = "Geri", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000018"), LanguageCode = "tr", Key = "common.next", Value = "İleri", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000019"), LanguageCode = "tr", Key = "common.close", Value = "Kapat", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0005-000000000020"), LanguageCode = "tr", Key = "common.actions", Value = "İşlemler", Category = "common", CreatedAt = createdAt },

        // === COMMON - English ===
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000001"), LanguageCode = "en", Key = "common.save", Value = "Save", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000002"), LanguageCode = "en", Key = "common.cancel", Value = "Cancel", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000003"), LanguageCode = "en", Key = "common.delete", Value = "Delete", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000004"), LanguageCode = "en", Key = "common.edit", Value = "Edit", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000005"), LanguageCode = "en", Key = "common.add", Value = "Add", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000006"), LanguageCode = "en", Key = "common.search", Value = "Search", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000007"), LanguageCode = "en", Key = "common.filter", Value = "Filter", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000008"), LanguageCode = "en", Key = "common.refresh", Value = "Refresh", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000009"), LanguageCode = "en", Key = "common.yes", Value = "Yes", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000010"), LanguageCode = "en", Key = "common.no", Value = "No", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000011"), LanguageCode = "en", Key = "common.loading", Value = "Loading...", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000012"), LanguageCode = "en", Key = "common.noData", Value = "No data found", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000013"), LanguageCode = "en", Key = "common.success", Value = "Success", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000014"), LanguageCode = "en", Key = "common.error", Value = "Error", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000015"), LanguageCode = "en", Key = "common.warning", Value = "Warning", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000016"), LanguageCode = "en", Key = "common.confirm", Value = "Confirm", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000017"), LanguageCode = "en", Key = "common.back", Value = "Back", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000018"), LanguageCode = "en", Key = "common.next", Value = "Next", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000019"), LanguageCode = "en", Key = "common.close", Value = "Close", Category = "common", CreatedAt = createdAt },
        new() { Id = Guid.Parse("40000000-0000-0000-0006-000000000020"), LanguageCode = "en", Key = "common.actions", Value = "Actions", Category = "common", CreatedAt = createdAt },
    };

        modelBuilder.Entity<TranslationEntity>().HasData(translations);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}