using Statement.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Commands;

public record RecordStatementPaymentCommand(RecordStatementPaymentDto Dto) : IRequest<Result<StatementPaymentResultDto>>;