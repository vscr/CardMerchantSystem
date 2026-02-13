namespace Fraud.Domain.Repositories;

using Fraud.Domain.Entities;

public interface ICardFraudProfileRepository
{
    Task<CardFraudProfile?> GetByCardNoAsync(string maskedCardNo, CancellationToken ct = default);
    Task<List<CardFraudProfile>> GetHighRiskCardsAsync(int minScore = 50, int top = 50, CancellationToken ct = default);
    Task AddAsync(CardFraudProfile profile, CancellationToken ct = default);
    Task UpdateAsync(CardFraudProfile profile, CancellationToken ct = default);

    /// <summary>Profil yoksa oluştur, varsa döndür</summary>
    Task<CardFraudProfile> GetOrCreateAsync(string maskedCardNo, CancellationToken ct = default);
}