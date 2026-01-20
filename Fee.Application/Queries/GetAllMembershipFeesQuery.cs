using Fee.Application.DTOs;
using Fee.Domain.Repositories;
using MediatR;

namespace Fee.Application.Queries;

public record GetAllMembershipFeesQuery() : IRequest<IReadOnlyList<MembershipFeeDto>>;
public class GetAllMembershipFeesQueryHandler
    : IRequestHandler<GetAllMembershipFeesQuery, IReadOnlyList<MembershipFeeDto>>
{
    private readonly IMembershipFeeRepository _repository;

    public GetAllMembershipFeesQueryHandler(IMembershipFeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MembershipFeeDto>> Handle(
        GetAllMembershipFeesQuery request,
        CancellationToken cancellationToken)
    {
        var fees = await _repository.GetAllActiveAsync(cancellationToken);

        return fees.Select(f => new MembershipFeeDto
        {
            Id = f.Id,
            FeeName = f.FeeName,
            FeeType = f.FeeType.Name,
            FeeTypeDisplayName = f.FeeType.DisplayName,
            Amount = f.Amount,
            Period = f.Period.Name,
            PeriodDisplayName = f.Period.DisplayName,
            GracePeriodDays = f.GracePeriodDays,
            LateFeeRate = f.LateFeeRate,
            IsActive = f.IsActive,
            Description = f.Description,
            MinimumTransactionVolume = f.MinimumTransactionVolume,
            MinimumTransactionCount = f.MinimumTransactionCount,
            CreatedAt = f.CreatedAt
        }).ToList();
    }
}