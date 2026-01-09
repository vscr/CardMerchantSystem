using BKM.Application.DTOs;
using MediatR;

namespace BKM.Application.Queries;

public record GetAllBINsQuery() : IRequest<IReadOnlyList<BINTableDto>>;