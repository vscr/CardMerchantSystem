using HSM.Application.DTOs;
using MediatR;

namespace HSM.Application.Queries;

public record GetCommandLogsQuery(DateTime StartDate, DateTime EndDate) : IRequest<IReadOnlyList<HSMCommandLogDto>>;