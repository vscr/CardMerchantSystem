using Transaction.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Commands;

/// <summary>
/// İade işlemi komutu
/// </summary>
public record RefundTransactionCommand(
    Guid OriginalTransactionId,
    decimal Amount,
    string OperatorUsername
) : IRequest<Result<TransactionResultDto>>;