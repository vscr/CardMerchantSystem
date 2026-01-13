using Accounting.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Queries;

public record GetTrialBalanceQuery(string PeriodCode) : IRequest<Result<TrialBalanceDto>>;