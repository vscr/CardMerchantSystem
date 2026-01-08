using Transaction.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Transaction.Application.Queries;

public record GetCardLimitQuery(string CardNumberMasked) : IRequest<Result<LimitInfoDto>>;