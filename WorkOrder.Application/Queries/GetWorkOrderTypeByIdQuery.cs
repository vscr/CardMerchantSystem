using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Queries;

public record GetWorkOrderTypeByIdQuery(Guid Id) : IRequest<WorkOrderTypeDto?>;

public class GetWorkOrderTypeByIdQueryHandler : IRequestHandler<GetWorkOrderTypeByIdQuery, WorkOrderTypeDto?>
{
    private readonly IWorkOrderTypeRepository _repository;

    public GetWorkOrderTypeByIdQueryHandler(IWorkOrderTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<WorkOrderTypeDto?> Handle(GetWorkOrderTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (type is null) return null;

        return new WorkOrderTypeDto
        {
            Id = type.Id,
            Code = type.Code,
            Name = type.Name,
            Description = type.Description,
            Category = type.Category.Name,
            CategoryDisplayName = type.Category.DisplayName,
            SlaHours = type.SlaHours,
            RequiresApproval = type.RequiresApproval,
            ApprovalLevels = type.ApprovalLevels,
            IsActive = type.IsActive,
            CreatedAt = type.CreatedAt
        };
    }
}