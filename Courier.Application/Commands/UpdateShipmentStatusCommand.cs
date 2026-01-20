using CardMerchantSystem.Shared.Kernel;
using Courier.Application.DTOs;
using Courier.Domain.Repositories;
using MediatR;

namespace Courier.Application.Commands;

public record UpdateShipmentStatusCommand(
    Guid ShipmentId,
    string StatusAction,
    string? Location,
    string OperatorUsername) : IRequest<Result<ShipmentDto>>;

public class UpdateShipmentStatusCommandHandler : IRequestHandler<UpdateShipmentStatusCommand, Result<ShipmentDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICourierCompanyRepository _companyRepository;

    public UpdateShipmentStatusCommandHandler(
        IShipmentRepository shipmentRepository,
        ICourierCompanyRepository companyRepository)
    {
        _shipmentRepository = shipmentRepository;
        _companyRepository = companyRepository;
    }

    public async Task<Result<ShipmentDto>> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdWithDetailsAsync(request.ShipmentId, cancellationToken);
        if (shipment is null)
            return Result.Failure<ShipmentDto>("Gönderi bulunamadı");

        Result result = request.StatusAction.ToLowerInvariant() switch
        {
            "intransit" => shipment.MarkAsInTransit(request.Location ?? "", request.OperatorUsername),
            "outfordelivery" => shipment.MarkAsOutForDelivery(request.OperatorUsername),
            "lost" => shipment.MarkAsLost(request.OperatorUsername),
            _ => Result.Failure($"Geçersiz durum aksiyonu: {request.StatusAction}")
        };

        if (result.IsFailure)
            return Result.Failure<ShipmentDto>(result.Error);

        _shipmentRepository.Update(shipment);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        var company = await _companyRepository.GetByIdAsync(shipment.CourierCompanyId, cancellationToken);

        return MapToDto(shipment, company?.Name ?? "", company?.GetTrackingUrl(shipment.TrackingNumber));
    }

    private static ShipmentDto MapToDto(Domain.Entities.Shipment shipment, string companyName, string? trackingUrl)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,
            ShipmentNumber = shipment.ShipmentNumber,
            TrackingNumber = shipment.TrackingNumber,
            Barcode = shipment.Barcode,
            CourierCompanyId = shipment.CourierCompanyId,
            CourierCompanyName = companyName,
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
            SmsNotificationSent = shipment.SmsNotificationSent,
            EmailNotificationSent = shipment.EmailNotificationSent,
            ShippingCost = shipment.ShippingCost,
            Notes = shipment.Notes,
            TrackingUrl = trackingUrl,
            DeliveryAttemptCount = shipment.DeliveryAttempts.Count,
            CreatedAt = shipment.CreatedAt
        };
    }
}