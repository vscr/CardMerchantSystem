using CardMerchantSystem.Shared.Kernel;
using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Enums;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Queries;

public record GetCardBlocksByStatusQuery(int StatusId) : IRequest<IReadOnlyList<CardBlockDto>>;

public class GetCardBlocksByStatusQueryHandler : IRequestHandler<GetCardBlocksByStatusQuery, IReadOnlyList<CardBlockDto>>
{
    private readonly ICardBlockRepository _repository;

    public GetCardBlocksByStatusQueryHandler(ICardBlockRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CardBlockDto>> Handle(GetCardBlocksByStatusQuery request, CancellationToken cancellationToken)
    {
        var status = Enumeration.FromId<BlockStatus>(request.StatusId);
        if (status is null)
            return new List<CardBlockDto>();

        var blocks = await _repository.GetByStatusAsync(status, cancellationToken);

        return blocks.Select(MapToDto).ToList();
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