using Courier.Application.DTOs;
using Courier.Domain.Entities;
using Courier.Domain.Repositories;
using MediatR;

namespace Courier.Application.Queries;

public record GetFailedDeliveriesQuery() : IRequest<IReadOnlyList<ShipmentDto>>;

public class GetFailedDeliveriesQueryHandler : IRequestHandler<GetFailedDeliveriesQuery, IReadOnlyList<ShipmentDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICourierCompanyRepository _companyRepository;

    public GetFailedDeliveriesQueryHandler(
        IShipmentRepository shipmentRepository,
        ICourierCompanyRepository companyRepository)
    {
        _shipmentRepository = shipmentRepository;
        _companyRepository = companyRepository;
    }

    public async Task<IReadOnlyList<ShipmentDto>> Handle(GetFailedDeliveriesQuery request, CancellationToken cancellationToken)
    {
        var shipments = await _shipmentRepository.GetFailedDeliveriesAsync(cancellationToken);
        var companies = await _companyRepository.GetAllAsync(cancellationToken);
        var companyDict = companies.ToDictionary(c => c.Id);

        return shipments.Select(s => MapToDto(s, companyDict.GetValueOrDefault(s.CourierCompanyId))).ToList();
    }

    private static ShipmentDto MapToDto(Shipment shipment, CourierCompany? company)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,
            ShipmentNumber = shipment.ShipmentNumber,
            TrackingNumber = shipment.TrackingNumber,
            Barcode = shipment.Barcode,
            CourierCompanyId = shipment.CourierCompanyId,
            CourierCompanyName = company?.Name ?? "",
            ShipmentType = shipment.ShipmentType.Name,
            ShipmentTypeDisplayName = shipment.ShipmentType.DisplayName,
            Status = shipment.Status.Name,
            StatusDisplayName = shipment.Status.DisplayName,
            CardApplicationId = shipment.CardApplicationId,
            PrintBatchItemId = shipment.PrintBatchItemId,
            RecipientName = shipment.RecipientName,
            RecipientPhone = shipment.RecipientPhone,
            RecipientEmail = shipment.RecipientEmail,
            DeliveryAddress = shipment.DeliveryAddress,
            DeliveryDistrict = shipment.DeliveryDistrict,
            DeliveryCity = shipment.DeliveryCity,
            DeliveryPostalCode = shipment.DeliveryPostalCode,
            PickedUpAt = shipment.PickedUpAt,
            DeliveredAt = shipment.DeliveredAt,
            ExpectedDeliveryDate = shipment.ExpectedDeliveryDate,
            DeliveredToName = shipment.DeliveredToName,
            SmsNotificationSent = shipment.SmsNotificationSent,
            EmailNotificationSent = shipment.EmailNotificationSent,
            ShippingCost = shipment.ShippingCost,
            Notes = shipment.Notes,
            TrackingUrl = company?.GetTrackingUrl(shipment.TrackingNumber),
            DeliveryAttemptCount = shipment.DeliveryAttempts.Count,
            CreatedAt = shipment.CreatedAt
        };
    }
}