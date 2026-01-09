using BKM.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Queries;

public record GetBINInfoQuery(string BIN) : IRequest<Result<BINTableDto>>;