using MediatR;
using Transaction.Application.DTOs;
using Transaction.Domain.Entities;
using Transaction.Domain.Repositories;

namespace Transaction.Application.Queries;

/// <summary>
/// Kart numarasına göre işlem listesi query'si
/// </summary>
public record GetTransactionsByCardQuery(string CardNumberMasked) : IRequest<IReadOnlyList<TransactionDto>>;

public class GetTransactionsByCardQueryHandler : IRequestHandler<GetTransactionsByCardQuery, IReadOnlyList<TransactionDto>>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionsByCardQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TransactionDto>> Handle(
        GetTransactionsByCardQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _repository.GetByCardNumberAsync(request.CardNumberMasked, cancellationToken);

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