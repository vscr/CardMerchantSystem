using Transaction.Application.DTOs;
using Transaction.Domain.Entities;
using Transaction.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Queries;

public class GetTransactionByIdQueryHandler
    : IRequestHandler<GetTransactionByIdQuery, Result<TransactionDto>>
{
    private readonly ITransactionRepository _repository;

    public GetTransactionByIdQueryHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<TransactionDto>> Handle(
        GetTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (transaction == null)
            return Result.Failure<TransactionDto>("İşlem bulunamadı", ErrorCodes.TransactionFailed);

        return MapToDto(transaction);
    }

    private static TransactionDto MapToDto(TransactionAggregate t)
    {
        return new TransactionDto
        {
            Id = t.Id,
            ReferenceNumber = t.ReferenceNumber.Value,
            TransactionType = t.TransactionType.Name,
            TransactionTypeDisplayName = t.TransactionType.DisplayName,
            Status = t.Status.Name,
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