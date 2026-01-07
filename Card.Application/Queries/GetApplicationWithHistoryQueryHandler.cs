using Card.Application.DTOs;
using Card.Domain.Entities;
using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Queries;

public class GetApplicationWithHistoryQueryHandler
    : IRequestHandler<GetApplicationWithHistoryQuery, Result<ApplicationWithHistoryDto>>
{
    private readonly ICardApplicationRepository _repository;

    public GetApplicationWithHistoryQueryHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ApplicationWithHistoryDto>> Handle(
        GetApplicationWithHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdWithHistoryAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<ApplicationWithHistoryDto>("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        var dto = new ApplicationWithHistoryDto
        {
            Application = MapToDto(application),
            StatusHistory = application.StatusHistory
                .OrderByDescending(h => h.ChangedAt)
                .Select(h => new StatusHistoryDto
                {
                    Id = h.Id,
                    Status = h.Status.Name,
                    StatusDisplayName = h.Status.DisplayName,
                    Description = h.Description,
                    ChangedBy = h.ChangedBy,
                    ChangedAt = h.ChangedAt
                })
                .ToList()
        };

        return dto;
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