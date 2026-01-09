using Campaign.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public record ApplyCampaignCommand(ApplyCampaignDto Dto) : IRequest<Result<ApplyCampaignResultDto>>;