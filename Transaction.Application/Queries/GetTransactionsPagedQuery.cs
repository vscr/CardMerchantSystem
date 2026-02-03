using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Microsoft.Extensions.Logging;
using Transaction.Application.DTOs;
using Transaction.Domain.Entities;
using Transaction.Domain.Enums;
using Transaction.Domain.Repositories;

namespace Transaction.Application.Queries;

/// <summary>
/// Sayfalı işlem listesi query'si
/// </summary>
public record GetTransactionsPagedQuery(TransactionFilterDto Filter) : IRequest<PagedResponse<TransactionDto>>;

public class GetTransactionsPagedQueryHandler : IRequestHandler<GetTransactionsPagedQuery, PagedResponse<TransactionDto>>
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger<GetTransactionsPagedQueryHandler> _logger;

    public GetTransactionsPagedQueryHandler(
        ITransactionRepository repository,
        ILogger<GetTransactionsPagedQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<TransactionDto>> Handle(
        GetTransactionsPagedQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching transactions page {PageNumber} with size {PageSize}",
            request.Filter.PageNumber,
            request.Filter.PageSize);

        // Status enum'ı çözümle
        TransactionStatus? status = null;
        if (request.Filter.StatusId.HasValue)
        {
            status = TransactionStatus.FromId<TransactionStatus>(request.Filter.StatusId.Value);
        }

        // TransactionType enum'ı çözümle
        TransactionType? transactionType = null;
        if (request.Filter.TransactionTypeId.HasValue)
        {
            transactionType = TransactionType.FromId<TransactionType>(request.Filter.TransactionTypeId.Value);
        }

        var (items, totalCount) = await _repository.GetPagedAsync(
            request.Filter.PageNumber,
            request.Filter.PageSize,
            status,
            transactionType,
            request.Filter.MerchantId,
            request.Filter.CardNumberMasked,
            request.Filter.StartDate,
            request.Filter.EndDate,
            request.Filter.SortBy,
            request.Filter.SortDescending,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        _logger.LogInformation(
            "Retrieved {Count} transactions out of {Total}",
            dtos.Count,
            totalCount);

        return PagedResponse<TransactionDto>.Create(
            dtos,
            totalCount,
            request.Filter.PageNumber,
            request.Filter.PageSize);
    }

    private static TransactionDto MapToDto(TransactionAggregate t)
    {
        return new TransactionDto
        {
            Id = t.Id,
            ReferenceNumber = t.ReferenceNumber.Value,
            TransactionType = t.TransactionType.Name,
            TransactionTypeId = t.TransactionType.Id,
            TransactionTypeDisplayName = t.TransactionType.DisplayName,
            Status = t.Status.Name,
            StatusId = t.Status.Id,
            StatusDisplayName = t.Status.DisplayName,
            Amount = t.Amount.Amount,
            Currency = t.Amount.Currency,
            AuthorizationCode = t.AuthorizationCode?.Value,
            CardNumberMasked = t.CardNumberMasked,
            MerchantId = t.MerchantId,
            MerchantCode = t.MerchantCode,
            TerminalId = t.TerminalId,
            TerminalCode = t.TerminalCode,
            DeclineReason = t.DeclineReason?.DisplayName,
            ErrorMessage = t.ErrorMessage,
            FraudCheckResult = t.FraudCheckResult?.DisplayName,
            FraudScore = t.FraudScore,
            OriginalTransactionId = t.OriginalTransactionId,
            SettledAt = t.SettledAt,
            BatchNumber = t.BatchNumber,
            CreatedAt = t.CreatedAt
        };
    }
}