namespace CardMerchantSystem.Shared.Kernel.Exceptions;

/// <summary>
/// Kaynak çakışması durumunda fırlatılır (duplicate vb.)
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message)
        : base(message, "CONFLICT")
    {
    }
}