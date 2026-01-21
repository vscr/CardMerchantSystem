using CardMerchantSystem.Shared.Kernel;
using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Commands;

public record CompleteWorkOrderCommand(Guid WorkOrderId, string Resolution, string CompletedBy) : IRequest<Result<WorkOrderItemDto>>;

public class CompleteWorkOrderCommandHandler : IRequestHandler<CompleteWorkOrderCommand, Result<WorkOrderItemDto>>
{
    private readonly IWorkOrderItemRepository _itemRepository;
    private readonly IWorkOrderTypeRepository _typeRepository;

    public CompleteWorkOrderCommandHandler(IWorkOrderItemRepository itemRepository, IWorkOrderTypeRepository typeRepository)
    {
        _itemRepository = itemRepository;
        _typeRepository = typeRepository;
    }

    public async Task<Result<WorkOrderItemDto>> Handle(CompleteWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdWithDetailsAsync(request.WorkOrderId, cancellationToken);
        if (item is null)
            return Result.Failure<WorkOrderItemDto>("İş emri bulunamadı");

        var result = item.Complete(request.Resolution, request.CompletedBy);
        if (result.IsFailure)
            return Result.Failure<WorkOrderItemDto>(result.Error);

        _itemRepository.Update(item);
        await _itemRepository.SaveChangesAsync(cancellationToken);

        var type = await _typeRepository.GetByIdAsync(item.WorkOrderTypeId, cancellationToken);
        return MapToDto(item, type!);
    }

    private static WorkOrderItemDto MapToDto(Domain.Entities.WorkOrderItem item, Domain.Entities.WorkOrderType type) => new()
    {
        Id = item.Id,
        OrderNumber = item.OrderNumber,
        WorkOrderTypeId = item.WorkOrderTypeId,
        TypeName = type.Name,
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