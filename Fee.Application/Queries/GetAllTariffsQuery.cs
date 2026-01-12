using Fee.Application.DTOs;
using MediatR;

namespace Fee.Application.Queries;

public record GetAllTariffsQuery() : IRequest<IReadOnlyList<TariffDto>>;