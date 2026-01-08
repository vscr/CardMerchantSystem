using Merchant.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Queries;

public record GetMerchantByIdQuery(Guid Id) : IRequest<Result<MerchantDto>>;