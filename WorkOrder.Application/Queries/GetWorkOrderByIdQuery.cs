using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Queries;

public record GetWorkOrderByIdQuery(Guid Id, bool IncludeDetails = false) : IRequest<WorkOrderItemDto?>;

public class GetWorkOrderByIdQueryHandler : IRequestHandler<GetWorkOrderByIdQuery, WorkOrderItemDto?>
{
    private readonly IWorkOrderItemRepository _itemRepository;
    private readonly IWorkOrderTypeRepository _typeRepository;

    public GetWorkOrderByIdQueryHandler(IWorkOrderItemRepository itemRepository, IWorkOrderTypeRepository typeRepository)
    {
        _itemRepository = itemRepository;
        _typeRepository = typeRepository;
    }

    public async Task<WorkOrderItemDto?> Handle(GetWorkOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var item = request.IncludeDetails
            ? await _itemRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            : await _itemRepository.GetByIdAsync(request.Id, cancellationToken);

        if (item is null) return null;

        var type = await _typeRepository.GetByIdAsync(item.WorkOrderTypeId, cancellationToken);

        if (request.IncludeDetails)
        {
            return new WorkOrderItemWithDetailsDto
            {
                Id = item.Id,
                OrderNumber = item.OrderNumber,
                WorkOrderTypeId = item.WorkOrderTypeId,
                TypeName = type!.Name,
                TypeCode = type.Code,
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
                CreatedAt = item.CreatedAt,
                Notes = item.Notes.OrderByDescending(n => n.CreatedAt).Select(n => new WorkOrderNoteDto
                {
                    Id = n.Id,
                    WorkOrderItemId = n.WorkOrderItemId,
                    Content = n.Content,
                    IsInternal = n.IsInternal,
                    CreatedAt = n.CreatedAt,
                    CreatedBy = n.CreatedBy
                }).ToList(),
                Approvals = item.Approvals.OrderBy(a => a.Level).Select(a => new WorkOrderApprovalDto
                {
                    Id = a.Id,
                    WorkOrderItemId = a.WorkOrderItemId,
                    Level = a.Level,
                    ApproverUsername = a.ApproverUsername,
                    Status = a.Status.Name,
                    StatusDisplayName = a.Status.DisplayName,
                    RequestedAt = a.RequestedAt,
                    ProcessedAt = a.ProcessedAt,
                    Notes = a.Notes
                }).ToList()
            };
        }

        return new WorkOrderItemDto
        {
            Id = item.Id,
            OrderNumber = item.OrderNumber,
            WorkOrderTypeId = item.WorkOrderTypeId,
            TypeName = type!.Name,
            TypeCode = type.Code,
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
        };
    }
}