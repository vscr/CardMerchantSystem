using Dispute.Application.DTOs;
using Dispute.Domain.Entities;
using Dispute.Domain.Enums;
using Dispute.Domain.Repositories;
using MediatR;

namespace Dispute.Application.Queries;

public record GetDisputesByStatusQuery(int StatusId) : IRequest<IReadOnlyList<DisputeDto>>;
public class GetDisputesByStatusQueryHandler
    : IRequestHandler<GetDisputesByStatusQuery, IReadOnlyList<DisputeDto>>
{
    private readonly IDisputeRepository _repository;

    public GetDisputesByStatusQueryHandler(IDisputeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<DisputeDto>> Handle(
        GetDisputesByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var status = DisputeStatus.FromId<DisputeStatus>(request.StatusId);

        if (status == null)
            return new List<DisputeDto>();

        var disputes = await _repository.GetByStatusAsync(status, cancellationToken);

        return disputes.Select(MapToDto).ToList();
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