namespace CardMerchantSystem.API.Auth.Entities;

public class MenuEntity
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;        // Unique key (dashboard, cards, etc.)
    public string Title { get; set; } = null!;       // Görünen isim (Dashboard, Kartlar)
    public string? Icon { get; set; }                // Lucide icon name (layout-dashboard, credit-card)
    public string? Path { get; set; }                // Route path (/dashboard, /cards)
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; } = true;      // Menüde görünsün mü
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public MenuEntity? Parent { get; set; }
    public ICollection<MenuEntity> Children { get; set; } = new List<MenuEntity>();
    public ICollection<MenuClaimEntity> Claims { get; set; } = new List<MenuClaimEntity>();
}