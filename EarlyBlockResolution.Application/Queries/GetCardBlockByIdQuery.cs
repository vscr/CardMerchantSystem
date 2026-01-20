using EarlyBlockResolution.Application.DTOs;
using EarlyBlockResolution.Domain.Entities;
using EarlyBlockResolution.Domain.Repositories;
using MediatR;

namespace EarlyBlockResolution.Application.Queries;

public record GetCardBlockByIdQuery(Guid Id, bool IncludeVerifications = false) : IRequest<CardBlockDto?>;

public class GetCardBlockByIdQueryHandler : IRequestHandler<GetCardBlockByIdQuery, CardBlockDto?>
{
    private readonly ICardBlockRepository _repository;

    public GetCardBlockByIdQueryHandler(ICardBlockRepository repository)
    {
        _repository = repository;
    }

    public async Task<CardBlockDto?> Handle(GetCardBlockByIdQuery request, CancellationToken cancellationToken)
    {
        var block = request.IncludeVerifications
            ? await _repository.GetByIdWithVerificationsAsync(request.Id, cancellationToken)
            : await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (block is null)
            return null;

        if (request.IncludeVerifications)
            return MapToDtoWithVerifications(block);

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

    private static CardBlockWithVerificationsDto MapToDtoWithVerifications(CardBlock block)
    {
        return new CardBlockWithVerificationsDto
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
            CreatedAt = block.CreatedAt,
            Verifications = block.Verifications.OrderByDescending(v => v.InitiatedAt).Select(v => new BlockVerificationDto
            {
                Id = v.Id,
                CardBlockId = v.CardBlockId,
                Method = v.Method.Name,
                MethodDisplayName = v.Method.DisplayName,
                Result = v.VerificationResult.Name,
                ResultDisplayName = v.VerificationResult.DisplayName,
                OtpSentAt = v.OtpSentAt,
                OtpExpiresAt = v.OtpExpiresAt,
                OtpAttempts = v.OtpAttempts,
                IsOtpValid = v.IsOtpValid(),
                InitiatedAt = v.InitiatedAt,
                CompletedAt = v.CompletedAt,
                Notes = v.Notes,
                AgentUsername = v.AgentUsername
            }).ToList()
        };
    }
}