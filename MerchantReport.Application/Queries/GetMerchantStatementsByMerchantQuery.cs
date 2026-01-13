using MerchantReport.Application.DTOs;
using MediatR;

namespace MerchantReport.Application.Queries;

public record GetMerchantStatementsByMerchantQuery(string MerchantId) : IRequest<IReadOnlyList<MerchantStatementSummaryDto>>;