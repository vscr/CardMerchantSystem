using Fraud.Domain.Enums;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Queries;

public record GetFraudAlertDetailQuery(Guid AlertId) : IRequest<FraudAlertDetailDto?>;

public class FraudAlertDetailDto
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
    public FraudAlertStatus Status { get; set; }
    public string? AssignedTo { get; set; }
    public FraudDecision? Decision { get; set; }
    public string? ResolutionNote { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // İlişkili veriler
    public List<HitScenarioDto> HitScenarios { get; set; } = new();
    public List<FraudActionDto> Actions { get; set; } = new();
    public CardFraudProfileDto? CardProfile { get; set; }
}

public class HitScenarioDto
{
    public Guid Id { get; set; }
    public Guid FraudScenarioId { get; set; }
    public string ScenarioName { get; set; } = null!;
    public int Score { get; set; }
    public string FraudResponseCode { get; set; } = null!;
    public bool IsSimulation { get; set; }
    public long ExecutionTimeMs { get; set; }
    public DateTime DetectedAt { get; set; }
}

public class FraudActionDto
{
    public Guid Id { get; set; }
    public FraudDecision Decision { get; set; }
    public string? Comment { get; set; }
    public string? CardStatusAction { get; set; }
    public string ActionBy { get; set; } = null!;
    public DateTime ActionAt { get; set; }
}

public class CardFraudProfileDto
{
    public int TotalTransactionCount { get; set; }
    public decimal TotalTransactionAmount { get; set; }
    public int TotalHitScenarioCount { get; set; }
    public int TotalFraudConfirmedCount { get; set; }
    public int CurrentRiskScore { get; set; }
    public DateTime? LastFraudAlertDate { get; set; }
    public DateTime? LastTransactionDate { get; set; }
}

public class GetFraudAlertDetailQueryHandler : IRequestHandler<GetFraudAlertDetailQuery, FraudAlertDetailDto?>
{
    private readonly IFraudAlertRepository _alertRepo;
    private readonly IHitScenarioRepository _hitRepo;
    private readonly IFraudActionRepository _actionRepo;
    private readonly ICardFraudProfileRepository _profileRepo;
    private readonly IFraudScenarioRepository _scenarioRepo;

    public GetFraudAlertDetailQueryHandler(
        IFraudAlertRepository alertRepo,
        IHitScenarioRepository hitRepo,
        IFraudActionRepository actionRepo,
        ICardFraudProfileRepository profileRepo,
        IFraudScenarioRepository scenarioRepo)
    {
        _alertRepo = alertRepo;
        _hitRepo = hitRepo;
        _actionRepo = actionRepo;
        _profileRepo = profileRepo;
        _scenarioRepo = scenarioRepo;
    }

    public async Task<FraudAlertDetailDto?> Handle(GetFraudAlertDetailQuery query, CancellationToken ct)
    {
        var alert = await _alertRepo.GetByIdAsync(query.AlertId, ct);
        if (alert == null) return null;

        var hits = await _hitRepo.GetByTransactionIdAsync(alert.TransactionId, ct);
        var actions = await _actionRepo.GetByAlertIdAsync(alert.Id, ct);
        var profile = await _profileRepo.GetByCardNoAsync(alert.MaskedCardNo, ct);
        var scenarios = await _scenarioRepo.GetAllAsync(ct);

        var scenarioLookup = scenarios.ToDictionary(s => s.Id, s => s.Name);

        return new FraudAlertDetailDto
        {
            Id = alert.Id,
            TransactionId = alert.TransactionId,
            MaskedCardNo = alert.MaskedCardNo,
            MerchantId = alert.MerchantId,
            MerchantName = alert.MerchantName,
            TransactionAmount = alert.TransactionAmount,
            CurrencyCode = alert.CurrencyCode,
            TotalScore = alert.TotalScore,
            HitScenarioCount = alert.HitScenarioCount,
            Status = alert.Status,
            AssignedTo = alert.AssignedTo,
            Decision = alert.Decision,
            ResolutionNote = alert.ResolutionNote,
            CreatedAt = alert.CreatedAt,
            ResolvedAt = alert.ResolvedAt,

            HitScenarios = hits.Select(h => new HitScenarioDto
            {
                Id = h.Id,
                FraudScenarioId = h.FraudScenarioId,
                ScenarioName = scenarioLookup.GetValueOrDefault(h.FraudScenarioId, "Bilinmeyen"),
                Score = h.Score,
                FraudResponseCode = h.FraudResponseCode,
                IsSimulation = h.IsSimulation,
                ExecutionTimeMs = h.ExecutionTimeMs,
                DetectedAt = h.DetectedAt
            }).ToList(),

            Actions = actions.Select(a => new FraudActionDto
            {
                Id = a.Id,
                Decision = a.Decision,
                Comment = a.Comment,
                CardStatusAction = a.CardStatusAction,
                ActionBy = a.ActionBy,
                ActionAt = a.CreatedAt
            }).ToList(),

            CardProfile = profile != null ? new CardFraudProfileDto
            {
                TotalTransactionCount = profile.TotalTransactionCount,
                TotalTransactionAmount = profile.TotalTransactionAmount,
                TotalHitScenarioCount = profile.TotalHitScenarioCount,
                TotalFraudConfirmedCount = profile.TotalFraudConfirmedCount,
                CurrentRiskScore = profile.CurrentRiskScore,
                LastFraudAlertDate = profile.LastFraudAlertDate,
                LastTransactionDate = profile.LastTransactionDate
            } : null
        };
    }
}