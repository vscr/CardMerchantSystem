using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Commands;

public record CreateFraudAlertCommand(CreateFraudAlertDto Dto) : IRequest<Result<FraudAlertDto>>;

public class CreateFraudAlertCommandHandler : IRequestHandler<CreateFraudAlertCommand, Result<FraudAlertDto>>
{
    private readonly IFraudAlertRepository _repository;

    public CreateFraudAlertCommandHandler(IFraudAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<FraudAlertDto>> Handle(CreateFraudAlertCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var reason = Enumeration.FromId<BlockReason>(dto.ReasonId);
        if (reason is null)
            return Result.Failure<FraudAlertDto>("Geçersiz bloke nedeni");

        var severity = Enumeration.FromId<AlertSeverity>(dto.SeverityId);
        if (severity is null)
            return Result.Failure<FraudAlertDto>("Geçersiz önem derecesi");

        var alertResult = FraudAlert.Create(
            dto.CardId,
            dto.CardNumberMasked,
            reason,
            severity,
            dto.FraudScore,
            dto.TransactionId,
            dto.TransactionAmount,
            dto.MerchantName,
            dto.BlockRuleId);

        if (alertResult.IsFailure)
            return Result.Failure<FraudAlertDto>(alertResult.Error);

        var alert = alertResult.Value!;

        await _repository.AddAsync(alert, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(alert);
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