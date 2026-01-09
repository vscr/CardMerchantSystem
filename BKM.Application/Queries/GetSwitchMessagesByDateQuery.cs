using BKM.Application.DTOs;
using MediatR;

namespace BKM.Application.Queries;

public record GetSwitchMessagesByDateQuery(DateTime StartDate, DateTime EndDate) : IRequest<IReadOnlyList<SwitchMessageDto>>;