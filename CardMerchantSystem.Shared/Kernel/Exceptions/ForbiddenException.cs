namespace CardMerchantSystem.Shared.Kernel.Exceptions;

/// <summary>
/// Erişim yasak durumunda fırlatılır
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Bu kaynağa erişim izniniz bulunmamaktadır")
        : base(message, "FORBIDDEN")
    {
    }
}