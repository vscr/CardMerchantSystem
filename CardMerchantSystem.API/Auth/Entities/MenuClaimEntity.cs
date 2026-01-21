namespace CardMerchantSystem.API.Auth.Entities;

public class MenuClaimEntity
{
    public Guid Id { get; set; }
    public Guid MenuId { get; set; }
    public string ClaimType { get; set; } = null!;   // "Role" veya "Policy"
    public string ClaimValue { get; set; } = null!;  // "Admin" veya "CardManagement"

    // Navigation
    public MenuEntity Menu { get; set; } = null!;
}