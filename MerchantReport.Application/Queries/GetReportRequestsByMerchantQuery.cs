using MerchantReport.Application.DTOs;
using MediatR;

namespace MerchantReport.Application.Queries;

public record GetReportRequestsByMerchantQuery(string MerchantId) : IRequest<IReadOnlyList<ReportRequestSummaryDto>>;