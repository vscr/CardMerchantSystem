using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Queries;

/// <summary>
/// Fraud dashboard özet istatistikleri.
/// Frontend'de ana sayfada gösterilecek KPI'lar.
/// </summary>
public record GetFraudDashboardQuery : IRequest<FraudDashboardDto>;

public class FraudDashboardDto
{
    public int NewAlertCount { get; set; }
    public int AssignedAlertCount { get; set; }
    public int InProgressAlertCount { get; set; }
    public int ResolvedTodayCount { get; set; }
    public int EscalatedAlertCount { get; set; }
    public List<CardFraudProfileDto> TopRiskyCards { get; set; } = new();
}

public class GetFraudDashboardQueryHandler : IRequestHandler<GetFraudDashboardQuery, FraudDashboardDto>
{
    private readonly IFraudAlertRepository _alertRepo;
    private readonly ICardFraudProfileRepository _profileRepo;

    public GetFraudDashboardQueryHandler(
        IFraudAlertRepository alertRepo,
        ICardFraudProfileRepository profileRepo)
    {
        _alertRepo = alertRepo;
        _profileRepo = profileRepo;
    }

    public async Task<FraudDashboardDto> Handle(GetFraudDashboardQuery query, CancellationToken ct)
    {
        var highRiskCards = await _profileRepo.GetHighRiskCardsAsync(50, 10, ct);

        return new FraudDashboardDto
        {
            NewAlertCount = await _alertRepo.GetCountByStatusAsync(FraudAlertStatus.New, ct),
            AssignedAlertCount = await _alertRepo.GetCountByStatusAsync(FraudAlertStatus.Assigned, ct),
            InProgressAlertCount = await _alertRepo.GetCountByStatusAsync(FraudAlertStatus.InProgress, ct),
            ResolvedTodayCount = await _alertRepo.GetCountByStatusAsync(FraudAlertStatus.Resolved, ct),
            EscalatedAlertCount = await _alertRepo.GetCountByStatusAsync(FraudAlertStatus.Escalated, ct),
            TopRiskyCards = highRiskCards.Select(p => new CardFraudProfileDto
            {
                TotalTransactionCount = p.TotalTransactionCount,
                TotalTransactionAmount = p.TotalTransactionAmount,
                TotalHitScenarioCount = p.TotalHitScenarioCount,
                TotalFraudConfirmedCount = p.TotalFraudConfirmedCount,
                CurrentRiskScore = p.CurrentRiskScore,
                LastFraudAlertDate = p.LastFraudAlertDate,
                LastTransactionDate = p.LastTransactionDate
            }).ToList()
        };
    }
}