namespace CardMerchantSystem.Shared.Kernel.Exceptions;

/// <summary>
/// Yetkilendirme hatalarında fırlatılır
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Bu işlem için yetkiniz bulunmamaktadır")
        : base(message, "UNAUTHORIZED")
    {
    }
}