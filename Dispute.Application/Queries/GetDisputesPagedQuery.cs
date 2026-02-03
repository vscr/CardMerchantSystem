using CardMerchantSystem.Shared.Kernel;
using Dispute.Application.DTOs;
using Dispute.Domain.Entities;
using Dispute.Domain.Enums;
using Dispute.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dispute.Application.Queries;

/// <summary>
/// Sayfalı itiraz listesi query'si
/// </summary>
public record GetDisputesPagedQuery(DisputeFilterDto Filter) : IRequest<PagedResponse<DisputeDto>>;

public class GetDisputesPagedQueryHandler : IRequestHandler<GetDisputesPagedQuery, PagedResponse<DisputeDto>>
{
    private readonly IDisputeRepository _repository;
    private readonly ILogger<GetDisputesPagedQueryHandler> _logger;

    public GetDisputesPagedQueryHandler(
        IDisputeRepository repository,
        ILogger<GetDisputesPagedQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<DisputeDto>> Handle(
        GetDisputesPagedQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching disputes page {PageNumber} with size {PageSize}",
            request.Filter.PageNumber,
            request.Filter.PageSize);

        // Status enum'ı çözümle
        DisputeStatus? status = null;
        if (request.Filter.StatusId.HasValue)
        {
            status = DisputeStatus.FromId<DisputeStatus>(request.Filter.StatusId.Value);
        }

        var (items, totalCount) = await _repository.GetPagedAsync(
            request.Filter.PageNumber,
            request.Filter.PageSize,
            status,
            request.Filter.MerchantId,
            request.Filter.CustomerTckn,
            request.Filter.StartDate,
            request.Filter.EndDate,
            request.Filter.IsOverdue,
            request.Filter.AssignedTo,
            request.Filter.SortBy,
            request.Filter.SortDescending,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        _logger.LogInformation(
            "Retrieved {Count} disputes out of {Total}",
            dtos.Count,
            totalCount);

        return PagedResponse<DisputeDto>.Create(
            dtos,
            totalCount,
            request.Filter.PageNumber,
            request.Filter.PageSize);
    }

    private static DisputeDto MapToDto(DisputeAggregate d)
    {
        return new DisputeDto
        {
            Id = d.Id,
            DisputeNumber = d.DisputeNumber,
            Status = d.Status.Name,
            StatusId = d.Status.Id,
            StatusDisplayName = d.Status.DisplayName,
            Reason = d.Reason.Name,
            ReasonId = d.Reason.Id,
            ReasonDisplayName = d.Reason.DisplayName,
            Priority = d.Priority.Name,
            PriorityId = d.Priority.Id,
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