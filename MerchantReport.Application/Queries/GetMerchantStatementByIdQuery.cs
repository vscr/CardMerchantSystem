using MerchantReport.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace MerchantReport.Application.Queries;

public record GetMerchantStatementByIdQuery(Guid Id) : IRequest<Result<MerchantStatementDto>>;