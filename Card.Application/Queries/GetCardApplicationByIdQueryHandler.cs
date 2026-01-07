using Card.Application.DTOs;
using Card.Domain.Entities;
using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Queries;

public class GetCardApplicationByIdQueryHandler
    : IRequestHandler<GetCardApplicationByIdQuery, Result<CardApplicationDto>>
{
    private readonly ICardApplicationRepository _repository;

    public GetCardApplicationByIdQueryHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CardApplicationDto>> Handle(
        GetCardApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<CardApplicationDto>("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

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