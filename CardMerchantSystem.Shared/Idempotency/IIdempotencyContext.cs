namespace CardMerchantSystem.Shared.Idempotency;

/// <summary>
/// HTTP request'ten gelen idempotency key'i 
/// MediatR pipeline'a taşıyan context.
/// Scoped olarak register edilir (per-request).
/// </summary>
public interface IIdempotencyContext
{
    string? IdempotencyKey { get; set; }
}

public class IdempotencyContext : IIdempotencyContext
{
    public string? IdempotencyKey { get; set; }
}