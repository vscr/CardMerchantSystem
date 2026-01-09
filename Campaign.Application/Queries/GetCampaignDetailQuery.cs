using Campaign.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Queries;

public record GetCampaignDetailQuery(Guid Id) : IRequest<Result<CampaignDetailDto>>;