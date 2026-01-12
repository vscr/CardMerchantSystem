using Fee.Application.DTOs;
using MediatR;

namespace Fee.Application.Queries;

public record GetFeeAccrualsByMerchantQuery(string MerchantId) : IRequest<IReadOnlyList<FeeAccrualDto>>;