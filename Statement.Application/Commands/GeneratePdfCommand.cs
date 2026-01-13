using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Commands;

public record GeneratePdfCommand(Guid StatementId) : IRequest<Result<byte[]>>;