using Campaign.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Queries;

public record GetCampaignByIdQuery(Guid Id) : IRequest<Result<CampaignDto>>;