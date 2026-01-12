using Fee.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Fee.Application.Commands;

public record CreateMembershipFeeCommand(CreateMembershipFeeDto Dto) : IRequest<Result<MembershipFeeDto>>;