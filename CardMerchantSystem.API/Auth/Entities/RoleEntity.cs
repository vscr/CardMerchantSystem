using CardMerchantSystem.API.Auth.Enums;

namespace CardMerchantSystem.API.Auth.Entities;

public class RoleEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? Description { get; set; }

    // Navigation
    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();

    // Seed helper
    public static RoleEntity FromEnum(SystemRole role) => new()
    {
        Id = (int)role,
        Name = role.ToString(),
        DisplayName = role switch
        {
            SystemRole.Admin => "Sistem Yöneticisi",
            SystemRole.CardOperator => "Kart Operasyon",
            SystemRole.MerchantOperator => "Üye İşyeri Operasyon",
            SystemRole.FinanceOperator => "Finans Operasyon",
            SystemRole.ComplianceOfficer => "Uyum Sorumlusu",
            SystemRole.CallCenterAgent => "Çağrı Merkezi",
            SystemRole.Viewer => "Görüntüleyici",
            _ => role.ToString()
        },
        Description = role switch
        {
            SystemRole.Admin => "Tüm sistem yetkilerine sahip",
            SystemRole.CardOperator => "Kart operasyonları yönetimi",
            SystemRole.MerchantOperator => "Üye işyeri operasyonları yönetimi",
            SystemRole.FinanceOperator => "Finansal işlemler yönetimi",
            SystemRole.ComplianceOfficer => "Uyum ve yasal raporlama",
            SystemRole.CallCenterAgent => "Çağrı merkezi işlemleri",
            SystemRole.Viewer => "Sadece görüntüleme yetkisi",
            _ => null
        }
    };
}