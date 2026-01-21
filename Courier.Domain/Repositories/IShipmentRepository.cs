using Courier.Domain.Entities;
using Courier.Domain.Enums;

namespace Courier.Domain.Repositories;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByShipmentNumberAsync(string shipmentNumber, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<Shipment?> GetByCardApplicationIdAsync(Guid cardApplicationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetByStatusAsync(ShipmentStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetByCourierCompanyIdAsync(Guid courierCompanyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetPendingDeliveriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetFailedDeliveriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Shipment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default);
    void Update(Shipment shipment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}