using Microsoft.EntityFrameworkCore;
using Transaction.Domain.Entities;
using Transaction.Domain.Repositories;
using Transaction.Infrastructure.Persistence;

namespace Transaction.Infrastructure.Repositories;

public class CardLimitDefinitionRepository : ICardLimitDefinitionRepository
{
    private readonly TransactionDbContext _context;

    public CardLimitDefinitionRepository(TransactionDbContext context) => _context = context;

    public async Task<CardLimitDefinition?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.CardLimitDefinitions.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<CardLimitDefinition?> GetDefaultAsync(CancellationToken ct = default)
        => await _context.CardLimitDefinitions
            .FirstOrDefaultAsync(x => x.LimitType == "DEFAULT" && x.IsActive, ct);

    public async Task<CardLimitDefinition?> GetByCardNoAsync(string maskedCardNo, CancellationToken ct = default)
        => await _context.CardLimitDefinitions
            .FirstOrDefaultAsync(x => x.LimitType == "CARD" && x.TargetValue == maskedCardNo && x.IsActive, ct);

    public async Task<CardLimitDefinition?> GetByBinAsync(string bin, CancellationToken ct = default)
        => await _context.CardLimitDefinitions
            .FirstOrDefaultAsync(x => x.LimitType == "BIN" && x.TargetValue == bin && x.IsActive, ct);

    public async Task<List<CardLimitDefinition>> GetAllAsync(CancellationToken ct = default)
        => await _context.CardLimitDefinitions
            .AsNoTracking()
            .OrderBy(x => x.LimitType)
            .ThenBy(x => x.TargetValue)
            .ToListAsync(ct);

    public async Task AddAsync(CardLimitDefinition definition, CancellationToken ct = default)
    {
        await _context.CardLimitDefinitions.AddAsync(definition, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(CardLimitDefinition definition, CancellationToken ct = default)
    {
        _context.CardLimitDefinitions.Update(definition);
        await _context.SaveChangesAsync(ct);
    }
}