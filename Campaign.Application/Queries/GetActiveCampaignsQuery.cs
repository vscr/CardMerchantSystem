using Campaign.Application.DTOs;
using MediatR;

namespace Campaign.Application.Queries;

public record GetActiveCampaignsQuery() : IRequest<IReadOnlyList<CampaignDto>>;