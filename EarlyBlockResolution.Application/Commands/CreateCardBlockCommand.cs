using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Commands;

public record CreateCardBlockCommand(CreateCardBlockDto Dto) : IRequest<Result<CardBlockDto>>;

public class CreateCardBlockCommandHandler : IRequestHandler<CreateCardBlockCommand, Result<CardBlockDto>>
{
    private readonly ICardBlockRepository _blockRepository;
    private readonly IFraudAlertRepository _alertRepository;

    public CreateCardBlockCommandHandler(
        ICardBlockRepository blockRepository,
        IFraudAlertRepository alertRepository)
    {
        _blockRepository = blockRepository;
        _alertRepository = alertRepository;
    }

    public async Task<Result<CardBlockDto>> Handle(CreateCardBlockCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Kartın aktif bloğu var mı kontrol et
        var existingBlock = await _blockRepository.GetActiveBlockByCardIdAsync(dto.CardId, cancellationToken);
        if (existingBlock is not null)
            return Result.Failure<CardBlockDto>("Bu kartın zaten aktif bir bloğu var");

        var reason = Enumeration.FromId<BlockReason>(dto.ReasonId);
        if (reason is null)
            return Result.Failure<CardBlockDto>("Geçersiz bloke nedeni");

        var severity = Enumeration.FromId<AlertSeverity>(dto.SeverityId);
        if (severity is null)
            return Result.Failure<CardBlockDto>("Geçersiz önem derecesi");

        var blockResult = CardBlock.Create(
            dto.CardId,
            dto.CardNumberMasked,
            dto.CustomerName,
            dto.CustomerPhone,
            dto.CustomerEmail,
            reason,
            severity,
            dto.BlockDurationMinutes,
            dto.FraudAlertId,
            dto.TransactionId);

        if (blockResult.IsFailure)
            return Result.Failure<CardBlockDto>(blockResult.Error);

        var block = blockResult.Value!;

        await _blockRepository.AddAsync(block, cancellationToken);

        // Fraud alert varsa güncelle
        if (dto.FraudAlertId.HasValue)
        {
            var alert = await _alertRepository.GetByIdAsync(dto.FraudAlertId.Value, cancellationToken);
            if (alert is not null)
            {
                alert.MarkAsProcessed(true, block.Id, "Bloke oluşturuldu");
                _alertRepository.Update(alert);
            }
        }

        await _blockRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(block);
    }

    private static CardBlockDto MapToDto(CardBlock block)
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