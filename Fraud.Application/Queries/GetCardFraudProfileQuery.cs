using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Queries;

public record GetCardFraudProfileQuery(string MaskedCardNo) : IRequest<CardFraudProfileDetailDto?>;

public class CardFraudProfileDetailDto
{
    public string MaskedCardNo { get; set; } = null!;
    public int TotalTransactionCount { get; set; }
    public decimal TotalTransactionAmount { get; set; }
    public int Last1HourTxCount { get; set; }
    public decimal Last1HourTxAmount { get; set; }
    public int Last24HourTxCount { get; set; }
    public decimal Last24HourTxAmount { get; set; }
    public int Last7DayTxCount { get; set; }
    public int Last24HourDistinctCountryCount { get; set; }
    public int Last24HourDistinctMerchantCount { get; set; }
    public string? LastTransactionCountry { get; set; }
    public string? LastMerchantId { get; set; }
    public int TotalHitScenarioCount { get; set; }
    public int TotalFraudConfirmedCount { get; set; }
    public int DeclinedTransactionCount { get; set; }
    public int CurrentRiskScore { get; set; }
    public DateTime? LastFraudAlertDate { get; set; }
    public DateTime? LastTransactionDate { get; set; }
    public DateTime? FirstTransactionDate { get; set; }
    public List<HitScenarioDto> RecentHitScenarios { get; set; } = new();
}

public class GetCardFraudProfileQueryHandler : IRequestHandler<GetCardFraudProfileQuery, CardFraudProfileDetailDto?>
{
    private readonly ICardFraudProfileRepository _profileRepo;
    private readonly IHitScenarioRepository _hitRepo;
    private readonly IFraudScenarioRepository _scenarioRepo;

    public GetCardFraudProfileQueryHandler(
        ICardFraudProfileRepository profileRepo,
        IHitScenarioRepository hitRepo,
        IFraudScenarioRepository scenarioRepo)
    {
        _profileRepo = profileRepo;
        _hitRepo = hitRepo;
        _scenarioRepo = scenarioRepo;
    }

    public async Task<CardFraudProfileDetailDto?> Handle(GetCardFraudProfileQuery query, CancellationToken ct)
    {
        var profile = await _profileRepo.GetByCardNoAsync(query.MaskedCardNo, ct);
        if (profile == null) return null;

        var recentHits = await _hitRepo.GetByCardNoAsync(query.MaskedCardNo, 30, ct);
        var scenarios = await _scenarioRepo.GetAllAsync(ct);
        var scenarioLookup = scenarios.ToDictionary(s => s.Id, s => s.Name);

        return new CardFraudProfileDetailDto
        {
            MaskedCardNo = profile.MaskedCardNo,
            TotalTransactionCount = profile.TotalTransactionCount,
            TotalTransactionAmount = profile.TotalTransactionAmount,
            Last1HourTxCount = profile.Last1HourTxCount,
            Last1HourTxAmount = profile.Last1HourTxAmount,
            Last24HourTxCount = profile.Last24HourTxCount,
            Last24HourTxAmount = profile.Last24HourTxAmount,
            Last7DayTxCount = profile.Last7DayTxCount,
            Last24HourDistinctCountryCount = profile.Last24HourDistinctCountryCount,
            Last24HourDistinctMerchantCount = profile.Last24HourDistinctMerchantCount,
            LastTransactionCountry = profile.LastTransactionCountry,
            LastMerchantId = profile.LastMerchantId,
            TotalHitScenarioCount = profile.TotalHitScenarioCount,
            TotalFraudConfirmedCount = profile.TotalFraudConfirmedCount,
            DeclinedTransactionCount = profile.DeclinedTransactionCount,
            CurrentRiskScore = profile.CurrentRiskScore,
            LastFraudAlertDate = profile.LastFraudAlertDate,
            LastTransactionDate = profile.LastTransactionDate,
            FirstTransactionDate = profile.FirstTransactionDate,
            RecentHitScenarios = recentHits.Select(h => new HitScenarioDto
            {
                Id = h.Id,
                FraudScenarioId = h.FraudScenarioId,
                ScenarioName = scenarioLookup.GetValueOrDefault(h.FraudScenarioId, "Bilinmeyen"),
                Score = h.Score,
                FraudResponseCode = h.FraudResponseCode,
                IsSimulation = h.IsSimulation,
                ExecutionTimeMs = h.ExecutionTimeMs,
                DetectedAt = h.DetectedAt
            }).ToList()
        };
    }
}