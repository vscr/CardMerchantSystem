using Accounting.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public record ClosePeriodCommand(string PeriodCode, string ClosedBy) : IRequest<Result<AccountingPeriodDto>>;