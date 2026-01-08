using CardMerchantSystem.Shared.Kernel;

namespace Merchant.Domain.Enums;

/// <summary>
/// Terminal durumları
/// </summary>
public class TerminalStatus : Enumeration
{
    public static readonly TerminalStatus Pending = new(1, nameof(Pending), "Kurulum Bekliyor");
    public static readonly TerminalStatus Active = new(2, nameof(Active), "Aktif");
    public static readonly TerminalStatus Inactive = new(3, nameof(Inactive), "Pasif");
    public static readonly TerminalStatus Maintenance = new(4, nameof(Maintenance), "Bakımda");
    public static readonly TerminalStatus Faulty = new(5, nameof(Faulty), "Arızalı");
    public static readonly TerminalStatus Removed = new(6, nameof(Removed), "Kaldırıldı");

    private TerminalStatus(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    public bool CanProcessTransactions => this == Active;

    public bool CanBeActivated => this == Pending || this == Inactive || this == Maintenance;
}