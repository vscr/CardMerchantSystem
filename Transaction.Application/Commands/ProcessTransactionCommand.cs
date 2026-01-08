using Transaction.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Commands;

/// <summary>
/// İşlem gerçekleştirme komutu (Ana işlem akışı)
/// </summary>
public record ProcessTransactionCommand(CreateTransactionDto Dto) : IRequest<Result<TransactionResultDto>>;