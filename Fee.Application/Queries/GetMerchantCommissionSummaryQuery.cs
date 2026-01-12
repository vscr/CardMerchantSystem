using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Queries;

public record GetMerchantCommissionSummaryQuery(
    string MerchantId,
    DateTime StartDate,
    DateTime EndDate) : IRequest<Result<MerchantCommissionSummaryDto>>;