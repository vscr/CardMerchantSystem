using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public record ActivateCampaignCommand(Guid CampaignId, string ApproverUsername) : IRequest<Result>;