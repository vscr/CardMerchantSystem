using Campaign.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public record CreateCampaignCommand(CreateCampaignDto Dto) : IRequest<Result<CampaignDto>>;