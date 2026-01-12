using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public record RecordPaymentCommand(RecordPaymentDto Dto) : IRequest<Result<FeeAccrualDto>>;