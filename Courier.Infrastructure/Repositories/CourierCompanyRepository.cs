using Courier.Domain.Entities;
using Courier.Domain.Repositories;
using Courier.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Courier.Infrastructure.Repositories;

public class CourierCompanyRepository : ICourierCompanyRepository
{
    private readonly CourierDbContext _context;

    public CourierCompanyRepository(CourierDbContext context)
    {
        _context = context;
    }

    public async Task<CourierCompany?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.CourierCompanies
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<CourierCompany?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.CourierCompanies
            .FirstOrDefaultAsync(x => x.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<CourierCompany>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CourierCompanies
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CourierCompany>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.CourierCompanies
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(CourierCompany company, CancellationToken cancellationToken = default)
    {
        await _context.CourierCompanies.AddAsync(company, cancellationToken);
    }

    public void Update(CourierCompany company)
    {
        _context.CourierCompanies.Update(company);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}