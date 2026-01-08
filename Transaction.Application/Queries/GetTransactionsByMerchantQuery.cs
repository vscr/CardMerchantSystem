using Transaction.Application.DTOs;
using MediatR;

namespace Transaction.Application.Queries;

public record GetTransactionsByMerchantQuery(Guid MerchantId) : IRequest<IReadOnlyList<TransactionDto>>;