using Card.Application.DTOs;
using Card.Domain.Repositories;
using MediatR;

namespace Card.Application.Queries;

public class GetAllCardApplicationsQueryHandler
    : IRequestHandler<GetAllCardApplicationsQuery, IReadOnlyList<CardApplicationDto>>
{
    private readonly ICardApplicationRepository _repository;

    public GetAllCardApplicationsQueryHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CardApplicationDto>> Handle(
        GetAllCardApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var applications = await _repository.GetAllAsync(cancellationToken);

        return applications.Select(a => new CardApplicationDto
        {
            Id = a.Id,
            CustomerTckn = a.CustomerTckn.Value,
            CustomerName = a.CustomerName,
            CustomerSurname = a.CustomerSurname,
            CustomerFullName = a.CustomerFullName,
            PhoneNumber = a.PhoneNumber,
            Email = a.Email,
            DeliveryAddress = a.DeliveryAddress.SingleLine,
            CardType = a.CardType.Name,
            Status = a.Status.Name,
            StatusDisplayName = a.Status.DisplayName,
            CardNumberMasked = a.CardNumberMasked,
            DailyLimit = a.DailyLimit.Amount,
            MonthlyLimit = a.MonthlyLimit.Amount,
            Currency = a.DailyLimit.Currency,
            PrintVendor = a.PrintVendor?.Name,
            PrintBatchId = a.PrintBatchId,
            PrintedAt = a.PrintedAt,
            CourierTrackingNumber = a.CourierTrackingNumber,
            DeliveredAt = a.DeliveredAt,
            ApprovedBy = a.ApprovedBy,
            ApprovedAt = a.ApprovedAt,
            RejectionReason = a.RejectionReason,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        }).ToList();
    }
}