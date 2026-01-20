using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Queries;

public record GetOverdueWorkOrdersQuery() : IRequest<IReadOnlyList<WorkOrderItemDto>>;

public class GetOverdueWorkOrdersQueryHandler : IRequestHandler<GetOverdueWorkOrdersQuery, IReadOnlyList<WorkOrderItemDto>>
{
    private readonly IWorkOrderItemRepository _itemRepository;
    private readonly IWorkOrderTypeRepository _typeRepository;

    public GetOverdueWorkOrdersQueryHandler(IWorkOrderItemRepository itemRepository, IWorkOrderTypeRepository typeRepository)
    {
        _itemRepository = itemRepository;
        _typeRepository = typeRepository;
    }

    public async Task<IReadOnlyList<WorkOrderItemDto>> Handle(GetOverdueWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var items = await _itemRepository.GetOverdueOrdersAsync(cancellationToken);
        var types = await _typeRepository.GetAllAsync(cancellationToken);
        var typeDict = types.ToDictionary(t => t.Id);

        return items.Select(item => new WorkOrderItemDto
        {
            Id = item.Id,
            OrderNumber = item.OrderNumber,
            WorkOrderTypeId = item.WorkOrderTypeId,
            TypeName = typeDict.GetValueOrDefault(item.WorkOrderTypeId)?.Name ?? "",
            TypeCode = typeDict.GetValueOrDefault(item.WorkOrderTypeId)?.Code ?? "",
            CardId = item.CardId,
            CustomerId = item.CustomerId,
            CustomerName = item.CustomerName,
            CustomerPhone = item.CustomerPhone,
            Subject = item.Subject,
            Description = item.Description,
            Status = item.Status.Name,
            StatusDisplayName = item.Status.DisplayName,
            Priority = item.Priority.Name,
            PriorityDisplayName = item.Priority.DisplayName,
            AssignedTo = item.AssignedTo,
            AssignedTeam = item.AssignedTeam,
            DueDate = item.DueDate,
            IsOverdue = item.IsOverdue,
            StartedAt = item.StartedAt,
            CompletedAt = item.CompletedAt,
            CompletedBy = item.CompletedBy,
            Resolution = item.Resolution,
            NoteCount = item.Notes.Count,
            ApprovalCount = item.Approvals.Count,
            CreatedAt = item.CreatedAt
        }).ToList();
    }
}