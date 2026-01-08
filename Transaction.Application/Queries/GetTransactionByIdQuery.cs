using Transaction.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Queries;

public record GetTransactionByIdQuery(Guid Id) : IRequest<Result<TransactionDto>>;