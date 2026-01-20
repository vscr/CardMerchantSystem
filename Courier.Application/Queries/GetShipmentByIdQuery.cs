using Courier.Application.DTOs;
using Courier.Domain.Entities;
using Courier.Domain.Repositories;
using MediatR;

namespace Courier.Application.Queries;

public record GetShipmentByIdQuery(Guid Id, bool IncludeDetails = false) : IRequest<ShipmentDto?>;

public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, ShipmentDto?>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICourierCompanyRepository _companyRepository;

    public GetShipmentByIdQueryHandler(
        IShipmentRepository shipmentRepository,
        ICourierCompanyRepository companyRepository)
    {
        _shipmentRepository = shipmentRepository;
        _companyRepository = companyRepository;
    }

    public async Task<ShipmentDto?> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
    {
        var shipment = request.IncludeDetails
            ? await _shipmentRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            : await _shipmentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (shipment is null)
            return null;

        var company = await _companyRepository.GetByIdAsync(shipment.CourierCompanyId, cancellationToken);

        if (request.IncludeDetails)
            return MapToDtoWithDetails(shipment, company);

        return MapToDto(shipment, company);
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

    private static ShipmentWithDetailsDto MapToDtoWithDetails(Shipment shipment, CourierCompany? company)
    {
        return new ShipmentWithDetailsDto
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
            CreatedAt = shipment.CreatedAt,
            StatusHistory = shipment.StatusHistory.OrderByDescending(h => h.OccurredAt).Select(h => new ShipmentStatusHistoryDto
            {
                Id = h.Id,
                ShipmentId = h.ShipmentId,
                Status = h.Status.Name,
                StatusDisplayName = h.Status.DisplayName,
                Description = h.Description,
                OccurredAt = h.OccurredAt,
                OperatorUsername = h.OperatorUsername,
                Location = h.Location
            }).ToList(),
            DeliveryAttempts = shipment.DeliveryAttempts.OrderByDescending(a => a.AttemptedAt).Select(a => new DeliveryAttemptDto
            {
                Id = a.Id,
                ShipmentId = a.ShipmentId,
                AttemptNumber = a.AttemptNumber,
                AttemptedAt = a.AttemptedAt,
                FailureReason = a.FailureReason.Name,
                FailureReasonDisplayName = a.FailureReason.DisplayName,
                Notes = a.Notes,
                CourierName = a.CourierName,
                CourierPhone = a.CourierPhone
            }).ToList()
        };
    }
}