using Card.Application.DTOs;
using Card.Domain.Entities;
using Card.Domain.Enums;
using Card.Domain.Repositories;
using Card.Domain.ValueObjects;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Yeni kart başvurusu oluşturma komutu
/// </summary>
public record CreateCardApplicationCommand(CreateCardApplicationDto Dto) : IRequest<Result<CardApplicationDto>>;
public class CreateCardApplicationCommandHandler
    : IRequestHandler<CreateCardApplicationCommand, Result<CardApplicationDto>>
{
    private readonly ICardApplicationRepository _repository;

    public CreateCardApplicationCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CardApplicationDto>> Handle(
        CreateCardApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // 1. TCKN oluştur ve doğrula
        var tcknResult = TCKN.Create(dto.CustomerTckn);
        if (tcknResult.IsFailure)
            return Result.Failure<CardApplicationDto>(tcknResult.Error!, tcknResult.ErrorCode);

        // 2. Aktif başvuru var mı kontrol et
        var hasActive = await _repository.HasActiveApplicationAsync(tcknResult.Value!, cancellationToken);
        if (hasActive)
            return Result.Failure<CardApplicationDto>(
                "Bu TCKN ile zaten aktif bir başvuru bulunmaktadır.",
                ErrorCodes.CardApplicationAlreadyExists);

        // 3. Adres oluştur
        var addressResult = Address.Create(
            dto.Street,
            dto.District,
            dto.City,
            dto.PostalCode,
            "Türkiye",
            dto.BuildingNo,
            dto.ApartmentNo);

        if (addressResult.IsFailure)
            return Result.Failure<CardApplicationDto>(addressResult.Error!, addressResult.ErrorCode);

        // 4. Kart tipini bul
        var cardType = CardType.FromId<CardType>(dto.CardTypeId);
        if (cardType == null)
            return Result.Failure<CardApplicationDto>("Geçersiz kart tipi", ErrorCodes.ValidationError);

        // 5. Limitleri ayarla
        Money? dailyLimit = null;
        Money? monthlyLimit = null;

        if (dto.DailyLimit.HasValue)
        {
            var dailyResult = Money.Create(dto.DailyLimit.Value);
            if (dailyResult.IsFailure)
                return Result.Failure<CardApplicationDto>(dailyResult.Error!, dailyResult.ErrorCode);
            dailyLimit = dailyResult.Value;
        }

        if (dto.MonthlyLimit.HasValue)
        {
            var monthlyResult = Money.Create(dto.MonthlyLimit.Value);
            if (monthlyResult.IsFailure)
                return Result.Failure<CardApplicationDto>(monthlyResult.Error!, monthlyResult.ErrorCode);
            monthlyLimit = monthlyResult.Value;
        }

        // 6. CardApplication oluştur
        var applicationResult = CardApplication.Create(
            tcknResult.Value!,
            dto.CustomerName,
            dto.CustomerSurname,
            dto.PhoneNumber,
            dto.Email,
            addressResult.Value!,
            cardType,
            dailyLimit,
            monthlyLimit);

        if (applicationResult.IsFailure)
            return Result.Failure<CardApplicationDto>(applicationResult.Error!, applicationResult.ErrorCode);

        var application = applicationResult.Value!;

        // 7. Kaydet
        await _repository.AddAsync(application, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        // 8. DTO'ya dönüştür ve dön
        return MapToDto(application);
    }

    private static CardApplicationDto MapToDto(CardApplication app)
    {
        return new CardApplicationDto
        {
            Id = app.Id,
            CustomerTckn = app.CustomerTckn.Masked,
            CustomerName = app.CustomerName,
            CustomerSurname = app.CustomerSurname,
            CustomerFullName = app.CustomerFullName,
            PhoneNumber = app.PhoneNumber,
            Email = app.Email,
            DeliveryAddress = app.DeliveryAddress.SingleLine,
            CardType = app.CardType.Name,
            Status = app.Status.Name,
            StatusDisplayName = app.Status.DisplayName,
            CardNumberMasked = app.CardNumberMasked,
            DailyLimit = app.DailyLimit.Amount,
            MonthlyLimit = app.MonthlyLimit.Amount,
            Currency = app.DailyLimit.Currency,
            PrintVendor = app.PrintVendor?.DisplayName,
            PrintBatchId = app.PrintBatchId,
            PrintedAt = app.PrintedAt,
            CourierTrackingNumber = app.CourierTrackingNumber,
            DeliveredAt = app.DeliveredAt,
            ApprovedBy = app.ApprovedBy,
            ApprovedAt = app.ApprovedAt,
            RejectionReason = app.RejectionReason,
            CreatedAt = app.CreatedAt,
            UpdatedAt = app.UpdatedAt
        };
    }
}