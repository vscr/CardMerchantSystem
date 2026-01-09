using BKM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Commands;

public record CreateBINCommand(CreateBINDto Dto) : IRequest<Result<BINTableDto>>;