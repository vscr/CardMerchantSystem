using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Queries;

public record GetFraudAlertsQuery(
    FraudAlertStatus? Status,
    string? AssignedTo,
    int Page = 1,
    int PageSize = 20
) : IRequest<FraudAlertsResult>;

public class FraudAlertsResult
{
    public List<FraudAlertDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class FraudAlertDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string MaskedCardNo { get; set; } = null!;
    public string? MerchantId { get; set; }
    public string? MerchantName { get; set; }
    public decimal TransactionAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public int TotalScore { get; set; }
    public int HitScenarioCount { get; set; }
    public string HighestFraudResponseCode { get; set; } = null!;
    public FraudAlertStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime? AssignedAt { get; set; }
    public FraudDecision? Decision { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class GetFraudAlertsQueryHandler : IRequestHandler<GetFraudAlertsQuery, FraudAlertsResult>
{
    private readonly IFraudAlertRepository _repo;

    public GetFraudAlertsQueryHandler(IFraudAlertRepository repo)
    {
        _repo = repo;
    }

    public async Task<FraudAlertsResult> Handle(GetFraudAlertsQuery query, CancellationToken ct)
    {
        List<FraudAlert> alerts;
        int totalCount;

        if (!string.IsNullOrEmpty(query.AssignedTo))
        {
            alerts = await _repo.GetAssignedToAsync(query.AssignedTo, ct);
            totalCount = alerts.Count;
        }
        else if (query.Status.HasValue)
        {
            alerts = await _repo.GetByStatusAsync(query.Status.Value, query.Page, query.PageSize, ct);
            totalCount = await _repo.GetCountByStatusAsync(query.Status.Value, ct);
        }
        else
        {
            alerts = await _repo.GetByStatusAsync(FraudAlertStatus.New, query.Page, query.PageSize, ct);
            totalCount = await _repo.GetCountByStatusAsync(FraudAlertStatus.New, ct);
        }

        return new FraudAlertsResult
        {
            Items = alerts.Select(a => new FraudAlertDto
            {
                Id = a.Id,
                TransactionId = a.TransactionId,
                MaskedCardNo = a.MaskedCardNo,
                MerchantId = a.MerchantId,
                MerchantName = a.MerchantName,
                TransactionAmount = a.TransactionAmount,
                CurrencyCode = a.CurrencyCode,
                TotalScore = a.TotalScore,
                HitScenarioCount = a.HitScenarioCount,
                HighestFraudResponseCode = a.HighestFraudResponseCode,
                Status = a.Status,
                AssignedTo = a.AssignedTo,
                AssignedAt = a.AssignedAt,
                Decision = a.Decision,
                CreatedAt = a.CreatedAt,
                ResolvedAt = a.ResolvedAt
            }).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}