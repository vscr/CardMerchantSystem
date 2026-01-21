using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Queries;

public record GetAllWorkOrderTypesQuery(bool ActiveOnly = false) : IRequest<IReadOnlyList<WorkOrderTypeDto>>;

public class GetAllWorkOrderTypesQueryHandler : IRequestHandler<GetAllWorkOrderTypesQuery, IReadOnlyList<WorkOrderTypeDto>>
{
    private readonly IWorkOrderTypeRepository _repository;

    public GetAllWorkOrderTypesQueryHandler(IWorkOrderTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<WorkOrderTypeDto>> Handle(GetAllWorkOrderTypesQuery request, CancellationToken cancellationToken)
    {
        var types = request.ActiveOnly
            ? await _repository.GetActiveAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return types.Select(t => new WorkOrderTypeDto
        {
            Id = t.Id,
            Code = t.Code,
            Name = t.Name,
            Description = t.Description,
            Category = t.Category.Name,
            CategoryDisplayName = t.Category.DisplayName,
            SlaHours = t.SlaHours,
            RequiresApproval = t.RequiresApproval,
            ApprovalLevels = t.ApprovalLevels,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt
        }).ToList();
    }
}