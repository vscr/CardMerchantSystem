using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Commands;

public record EscalateBlockCommand(
    Guid CardBlockId,
    string Reason,
    string OperatorUsername) : IRequest<Result<CardBlockDto>>;

public class EscalateBlockCommandHandler : IRequestHandler<EscalateBlockCommand, Result<CardBlockDto>>
{
    private readonly ICardBlockRepository _repository;

    public EscalateBlockCommandHandler(ICardBlockRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CardBlockDto>> Handle(EscalateBlockCommand request, CancellationToken cancellationToken)
    {
        var block = await _repository.GetByIdWithVerificationsAsync(request.CardBlockId, cancellationToken);
        if (block is null)
            return Result.Failure<CardBlockDto>("Bloke bulunamadı");

        var escalateResult = block.Escalate(request.Reason, request.OperatorUsername);
        if (escalateResult.IsFailure)
            return Result.Failure<CardBlockDto>(escalateResult.Error);

        _repository.Update(block);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(block);
    }

    private static CardBlockDto MapToDto(Domain.Entities.CardBlock block)
    {
        return new CardBlockDto
        {
            Id = block.Id,
            BlockNumber = block.BlockNumber,
            CardId = block.CardId,
            CardNumberMasked = block.CardNumberMasked,
            CustomerName = block.CustomerName,
            CustomerPhone = block.CustomerPhone,
            CustomerEmail = block.CustomerEmail,
            Reason = block.Reason.Name,
            ReasonDisplayName = block.Reason.DisplayName,
            Status = block.Status.Name,
            StatusDisplayName = block.Status.DisplayName,
            Severity = block.Severity.Name,
            SeverityDisplayName = block.Severity.DisplayName,
            FraudAlertId = block.FraudAlertId,
            TransactionId = block.TransactionId,
            BlockedAt = block.BlockedAt,
            ExpiresAt = block.ExpiresAt,
            ResolvedAt = block.ResolvedAt,
            ResolvedBy = block.ResolvedBy,
            ResolutionNotes = block.ResolutionNotes,
            SmsNotificationSent = block.SmsNotificationSent,
            EmailNotificationSent = block.EmailNotificationSent,
            IsBlockActive = block.IsBlockActive(),
            VerificationCount = block.Verifications.Count,
            CreatedAt = block.CreatedAt
        };
    }
}