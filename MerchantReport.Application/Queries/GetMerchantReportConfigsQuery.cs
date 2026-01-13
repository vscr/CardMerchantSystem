using MerchantReport.Application.DTOs;
using MediatR;

namespace MerchantReport.Application.Queries;

public record GetMerchantReportConfigsQuery(string MerchantId) : IRequest<IReadOnlyList<MerchantReportConfigDto>>;