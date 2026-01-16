using Card.Application.DTOs;
using MediatR;

namespace Card.Application.Queries;

public record GetAllCardApplicationsQuery() : IRequest<IReadOnlyList<CardApplicationDto>>;