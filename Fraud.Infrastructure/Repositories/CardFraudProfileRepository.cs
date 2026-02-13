using Fraud.Domain.Entities;
using Fraud.Domain.Repositories;
using Fraud.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Repositories;

public class CardFraudProfileRepository : ICardFraudProfileRepository
{
    private readonly FraudDbContext _context;

    public CardFraudProfileRepository(FraudDbContext context) => _context = context;

    public async Task<CardFraudProfile?> GetByCardNoAsync(string maskedCardNo, CancellationToken ct = default)
        => await _context.CardFraudProfiles
            .FirstOrDefaultAsync(x => x.MaskedCardNo == maskedCardNo, ct);

    public async Task<List<CardFraudProfile>> GetHighRiskCardsAsync(int minScore = 50, int top = 50, CancellationToken ct = default)
        => await _context.CardFraudProfiles
            .AsNoTracking()
            .Where(x => x.CurrentRiskScore >= minScore)
            .OrderByDescending(x => x.CurrentRiskScore)
            .Take(top)
            .ToListAsync(ct);

    public async Task AddAsync(CardFraudProfile profile, CancellationToken ct = default)
    {
        await _context.CardFraudProfiles.AddAsync(profile, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CardFraudProfile profile, CancellationToken ct = default)
    {
        _context.CardFraudProfiles.Update(profile);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<CardFraudProfile> GetOrCreateAsync(string maskedCardNo, CancellationToken ct = default)
    {
        var profile = await GetByCardNoAsync(maskedCardNo, ct);
        if (profile != null) return profile;

        profile = new CardFraudProfile(maskedCardNo);
        await _context.CardFraudProfiles.AddAsync(profile, ct);
        await _context.SaveChangesAsync(ct);
        return profile;
    }
}