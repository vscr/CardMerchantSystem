using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Queries;

public record GetUnprocessedFraudAlertsQuery() : IRequest<IReadOnlyList<FraudAlertDto>>;

public class GetUnprocessedFraudAlertsQueryHandler : IRequestHandler<GetUnprocessedFraudAlertsQuery, IReadOnlyList<FraudAlertDto>>
{
    private readonly IFraudAlertRepository _repository;

    public GetUnprocessedFraudAlertsQueryHandler(IFraudAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<FraudAlertDto>> Handle(GetUnprocessedFraudAlertsQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _repository.GetUnprocessedAsync(cancellationToken);

        return alerts.Select(MapToDto).ToList();
    }

    private static FraudAlertDto MapToDto(FraudAlert alert)
    {
        return new FraudAlertDto
        {
            Id = alert.Id,
            AlertNumber = alert.AlertNumber,
            CardId = alert.CardId,
            CardNumberMasked = alert.CardNumberMasked,
            TransactionId = alert.TransactionId,
            TransactionAmount = alert.TransactionAmount,
            MerchantName = alert.MerchantName,
            Reason = alert.Reason.Name,
            ReasonDisplayName = alert.Reason.DisplayName,
            Severity = alert.Severity.Name,
            SeverityDisplayName = alert.Severity.DisplayName,
            FraudScore = alert.FraudScore,
            BlockRuleId = alert.BlockRuleId,
            IsProcessed = alert.IsProcessed,
            BlockCreated = alert.BlockCreated,
            CardBlockId = alert.CardBlockId,
            DetectedAt = alert.DetectedAt,
            ProcessedAt = alert.ProcessedAt,
            Notes = alert.Notes,
            CreatedAt = alert.CreatedAt
        };
    }
}