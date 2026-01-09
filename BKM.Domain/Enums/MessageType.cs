using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Enums;

/// <summary>
/// ISO 8583 Mesaj Tipleri
/// </summary>
public class MessageType : Enumeration
{
    public static readonly MessageType Authorization = new(1, "0100", "Authorization Request");
    public static readonly MessageType AuthorizationResponse = new(2, "0110", "Authorization Response");
    public static readonly MessageType Financial = new(3, "0200", "Financial Request");
    public static readonly MessageType FinancialResponse = new(4, "0210", "Financial Response");
    public static readonly MessageType Reversal = new(5, "0400", "Reversal Request");
    public static readonly MessageType ReversalResponse = new(6, "0410", "Reversal Response");
    public static readonly MessageType NetworkManagement = new(7, "0800", "Network Management Request");
    public static readonly MessageType NetworkManagementResponse = new(8, "0810", "Network Management Response");

    private MessageType(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Request mesajı mı?
    /// </summary>
    public bool IsRequest => Name.EndsWith("00");

    /// <summary>
    /// Response mesajı mı?
    /// </summary>
    public bool IsResponse => Name.EndsWith("10");
}