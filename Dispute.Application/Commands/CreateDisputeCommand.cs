using CardMerchantSystem.Shared.Kernel;
using Dispute.Application.DTOs;
using Dispute.Domain.Entities;
using Dispute.Domain.Enums;
using Dispute.Domain.Repositories;
using MediatR;

namespace Dispute.Application.Commands;

public record CreateDisputeCommand(CreateDisputeDto Dto) : IRequest<Result<DisputeDto>>;
public class CreateDisputeCommandHandler
    : IRequestHandler<CreateDisputeCommand, Result<DisputeDto>>
{
    private readonly IDisputeRepository _repository;

    public CreateDisputeCommandHandler(IDisputeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DisputeDto>> Handle(
        CreateDisputeCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // 1. Reason bul
        var reason = DisputeReason.FromId<DisputeReason>(dto.ReasonId);
        if (reason == null)
            return Result.Failure<DisputeDto>("Geçersiz itiraz nedeni", ErrorCodes.ValidationError);

        // 2. Dispute oluştur
        var disputeResult = DisputeAggregate.Create(
            dto.TransactionId,
            dto.TransactionReference,
            dto.TransactionAmount,
            dto.DisputedAmount,
            dto.TransactionDate,
            reason,
            dto.Description,
            dto.CustomerTckn,
            dto.CustomerName,
            dto.CustomerPhone,
            dto.CustomerEmail,
            dto.MerchantId,
            dto.MerchantCode,
            dto.MerchantName);

        if (disputeResult.IsFailure)
            return Result.Failure<DisputeDto>(disputeResult.Error!, disputeResult.ErrorCode);

        var dispute = disputeResult.Value!;

        // 3. Müşteri beyanı varsa ekle
        if (!string.IsNullOrWhiteSpace(dto.CustomerStatement))
        {
            dispute.AddCustomerStatement(dto.CustomerStatement, "System");
        }

        // 4. Kaydet
        await _repository.AddAsync(dispute, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(dispute);
    }

    private static DisputeDto MapToDto(DisputeAggregate d)
    {
        return new DisputeDto
        {
            Id = d.Id,
            DisputeNumber = d.DisputeNumber,
            Status = d.Status.Name,
            StatusDisplayName = d.Status.DisplayName,
            Reason = d.Reason.Name,
            ReasonDisplayName = d.Reason.DisplayName,
            Priority = d.Priority.Name,
            PriorityDisplayName = d.Priority.DisplayName,
            TransactionId = d.TransactionId,
            TransactionReference = d.TransactionReference,
            TransactionAmount = d.TransactionAmount,
            DisputedAmount = d.DisputedAmount,
            TransactionDate = d.TransactionDate,
            CustomerName = d.CustomerName,
            CustomerPhone = d.CustomerPhone,
            CustomerEmail = d.CustomerEmail,
            MerchantId = d.MerchantId,
            MerchantCode = d.MerchantCode,
            MerchantName = d.MerchantName,
            Description = d.Description,
            CustomerStatement = d.CustomerStatement,
            MerchantResponse = d.MerchantResponse,
            MerchantResponseDate = d.MerchantResponseDate,
            AssignedTo = d.AssignedTo,
            AssignedAt = d.AssignedAt,
            Resolution = d.Resolution,
            ResolvedAt = d.ResolvedAt,
            ResolvedBy = d.ResolvedBy,
            RefundAmount = d.RefundAmount,
            DueDate = d.DueDate,
            CreatedAt = d.CreatedAt,
            IsOverdue = d.IsOverdue,
            RemainingDays = d.RemainingDays,
            DocumentCount = d.Documents.Count,
            NoteCount = d.Notes.Count
        };
    }
}