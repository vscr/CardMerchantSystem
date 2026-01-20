using CardMerchantSystem.Shared.Kernel;
using Courier.Application.DTOs;
using Courier.Domain.Entities;
using Courier.Domain.Enums;
using Courier.Domain.Repositories;
using MediatR;

namespace Courier.Application.Commands;

public record CreateShipmentCommand(CreateShipmentDto Dto) : IRequest<Result<ShipmentDto>>;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Result<ShipmentDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICourierCompanyRepository _companyRepository;

    public CreateShipmentCommandHandler(
        IShipmentRepository shipmentRepository,
        ICourierCompanyRepository companyRepository)
    {
        _shipmentRepository = shipmentRepository;
        _companyRepository = companyRepository;
    }

    public async Task<Result<ShipmentDto>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var company = await _companyRepository.GetByIdAsync(dto.CourierCompanyId, cancellationToken);
        if (company is null)
            return Result.Failure<ShipmentDto>("Kurye firması bulunamadı");

        if (!company.IsActive)
            return Result.Failure<ShipmentDto>("Kurye firması aktif değil");

        var shipmentType = Enumeration.FromId<ShipmentType>(dto.ShipmentTypeId);
        if (shipmentType is null)
            return Result.Failure<ShipmentDto>("Geçersiz gönderi tipi");

        var shipmentResult = Shipment.Create(
            dto.CourierCompanyId,
            shipmentType,
            dto.RecipientName,
            dto.RecipientPhone,
            dto.RecipientEmail,
            dto.RecipientTckn,
            dto.DeliveryAddress,
            dto.DeliveryDistrict,
            dto.DeliveryCity,
            dto.DeliveryPostalCode,
            company.StandardDeliveryDays,
            company.BasePrice,
            dto.CardApplicationId,
            dto.PrintBatchItemId);

        if (shipmentResult.IsFailure)
            return Result.Failure<ShipmentDto>(shipmentResult.Error);

        var shipment = shipmentResult.Value!;

        await _shipmentRepository.AddAsync(shipment, cancellationToken);
        await _shipmentRepository.SaveChangesAsync(cancellationToken);

        return new ShipmentDto
        {
            Id = shipment.Id,
            ShipmentNumber = shipment.ShipmentNumber,
            TrackingNumber = shipment.TrackingNumber,
            Barcode = shipment.Barcode,
            CourierCompanyId = shipment.CourierCompanyId,
            CourierCompanyName = company.Name,
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
            TrackingUrl = company.GetTrackingUrl(shipment.TrackingNumber),
            DeliveryAttemptCount = 0,
            CreatedAt = shipment.CreatedAt
        };
    }
}