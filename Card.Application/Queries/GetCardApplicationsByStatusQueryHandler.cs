using Card.Application.DTOs;
using Card.Domain.Entities;
using Card.Domain.Enums;
using Card.Domain.Repositories;
using MediatR;

namespace Card.Application.Queries;

public class GetCardApplicationsByStatusQueryHandler
    : IRequestHandler<GetCardApplicationsByStatusQuery, IReadOnlyList<CardApplicationDto>>
{
    private readonly ICardApplicationRepository _repository;

    public GetCardApplicationsByStatusQueryHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CardApplicationDto>> Handle(
        GetCardApplicationsByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var status = CardApplicationStatus.FromId<CardApplicationStatus>(request.StatusId);

        if (status == null)
            return new List<CardApplicationDto>();

        var applications = await _repository.GetByStatusAsync(status, cancellationToken);

        return applications.Select(MapToDto).ToList();
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