using HSM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace HSM.Application.Commands;

public record GenerateKeyCommand(CreateHSMKeyDto Dto) : IRequest<Result<HSMKeyDto>>;