using Courier.Domain.Entities;
using Courier.Domain.Enums;
using Courier.Domain.Repositories;
using Courier.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Courier.Infrastructure.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly CourierDbContext _context;

    public ShipmentRepository(CourierDbContext context)
    {
        _context = context;
    }

    public async Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Shipment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.StatusHistory)
            .Include(x => x.DeliveryAttempts)
            .Include(x => x.Company)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Shipment?> GetByShipmentNumberAsync(string shipmentNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .FirstOrDefaultAsync(x => x.ShipmentNumber == shipmentNumber, cancellationToken);
    }

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.StatusHistory)
            .Include(x => x.DeliveryAttempts)
            .FirstOrDefaultAsync(x => x.TrackingNumber == trackingNumber, cancellationToken);
    }

    public async Task<Shipment?> GetByCardApplicationIdAsync(Guid cardApplicationId, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.StatusHistory)
            .FirstOrDefaultAsync(x => x.CardApplicationId == cardApplicationId, cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetByStatusAsync(ShipmentStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.DeliveryAttempts)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetByCourierCompanyIdAsync(Guid courierCompanyId, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.DeliveryAttempts)
            .Where(x => x.CourierCompanyId == courierCompanyId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetPendingDeliveriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.DeliveryAttempts)
            .Where(x => x.Status == ShipmentStatus.Created ||
                        x.Status == ShipmentStatus.PickedUp ||
                        x.Status == ShipmentStatus.InTransit ||
                        x.Status == ShipmentStatus.OutForDelivery)
            .OrderBy(x => x.ExpectedDeliveryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetFailedDeliveriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.DeliveryAttempts)
            .Where(x => x.Status == ShipmentStatus.DeliveryFailed)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Shipment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Shipments
            .Include(x => x.DeliveryAttempts)
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        await _context.Shipments.AddAsync(shipment, cancellationToken);
    }

    public void Update(Shipment shipment)
    {
        _context.Shipments.Update(shipment);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}