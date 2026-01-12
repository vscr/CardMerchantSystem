using Fee.Application.DTOs;
using MediatR;

namespace Fee.Application.Queries;

public record GetAllMembershipFeesQuery() : IRequest<IReadOnlyList<MembershipFeeDto>>;