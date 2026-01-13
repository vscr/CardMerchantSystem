using Statement.Domain.Entities;
using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Services;

/// <summary>
/// Ekstre Oluşturma Servisi
/// </summary>
public interface IStatementGenerationService
{
    /// <summary>
    /// Kart için ekstre oluşturur
    /// </summary>
    Task<Result<CardStatement>> GenerateStatementAsync(
        string cardNumber,
        DateTime periodEndDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Toplu ekstre oluşturur (belirli kesim günü için)
    /// </summary>
    Task<Result<int>> GenerateBatchStatementsAsync(
        int statementDay,
        CancellationToken cancellationToken = default);
}