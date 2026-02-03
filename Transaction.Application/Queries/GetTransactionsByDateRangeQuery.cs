using MediatR;
using Transaction.Application.DTOs;
using Transaction.Domain.Entities;
using Transaction.Domain.Repositories;

namespace Transaction.Application.Queries;

/// <summary>
/// Tarih aralığına göre işlem listesi query'si
/// </summary>
public record GetTransactionsByDateRangeQuery(DateTime StartDate, DateTime EndDate) : IRequest<IReadOnlyList<TransactionDto>>;

public class GetTransactionsByDateRangeQueryHandler : IRequestHandler<GetTransactionsByDateRangeQuery, IReadOnlyList<TransactionDto>>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionsByDateRangeQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TransactionDto>> Handle(
        GetTransactionsByDateRangeQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _repository.GetByDateRangeAsync(
            request.StartDate,
            request.EndDate,
            cancellationToken);

        return transactions.Select(MapToDto).ToList();
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