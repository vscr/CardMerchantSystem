using Accounting.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public record CreateAccountingPeriodCommand(CreateAccountingPeriodDto Dto) : IRequest<Result<AccountingPeriodDto>>;