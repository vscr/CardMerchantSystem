using Accounting.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public record CreateChartOfAccountCommand(CreateChartOfAccountDto Dto) : IRequest<Result<ChartOfAccountDto>>;