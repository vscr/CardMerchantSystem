using HSM.Application.DTOs;
using MediatR;

namespace HSM.Application.Queries;

public record GetAllHSMKeysQuery() : IRequest<IReadOnlyList<HSMKeyDto>>;