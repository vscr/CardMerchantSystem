using CardMerchantSystem.Shared.Kernel;

namespace Dispute.Domain.Enums;

/// <summary>
/// İtiraz nedenleri
/// </summary>
public class DisputeReason : Enumeration
{
    public static readonly DisputeReason UnauthorizedTransaction = new(1, nameof(UnauthorizedTransaction), "Yetkisiz İşlem");
    public static readonly DisputeReason DuplicateTransaction = new(2, nameof(DuplicateTransaction), "Mükerrer İşlem");
    public static readonly DisputeReason ProductNotReceived = new(3, nameof(ProductNotReceived), "Ürün/Hizmet Alınmadı");
    public static readonly DisputeReason ProductNotAsDescribed = new(4, nameof(ProductNotAsDescribed), "Ürün Tanımlandığı Gibi Değil");
    public static readonly DisputeReason CreditNotProcessed = new(5, nameof(CreditNotProcessed), "İade İşlenmedi");
    public static readonly DisputeReason IncorrectAmount = new(6, nameof(IncorrectAmount), "Yanlış Tutar");
    public static readonly DisputeReason CancelledService = new(7, nameof(CancelledService), "İptal Edilen Hizmet");
    public static readonly DisputeReason AtmWithdrawalIssue = new(8, nameof(AtmWithdrawalIssue), "ATM Çekim Sorunu");
    public static readonly DisputeReason Other = new(9, nameof(Other), "Diğer");

    private DisputeReason(int id, string name, string displayName)
        : base(id, name, displayName)
    {
    }

    /// <summary>
    /// Belge gerektiren itiraz mı?
    /// </summary>
    public bool RequiresDocumentation => this == ProductNotReceived ||
                                          this == ProductNotAsDescribed ||
                                          this == CreditNotProcessed;

    /// <summary>
    /// Maksimum çözüm süresi (gün)
    /// </summary>
    public int MaxResolutionDays => this switch
    {
        _ when this == UnauthorizedTransaction => 10,
        _ when this == AtmWithdrawalIssue => 5,
        _ => 30
    };
}