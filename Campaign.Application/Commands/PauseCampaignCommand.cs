using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Campaign.Application.Commands;

public record PauseCampaignCommand(Guid CampaignId, string Reason, string Username) : IRequest<Result>;