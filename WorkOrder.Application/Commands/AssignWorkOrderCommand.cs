using CardMerchantSystem.Shared.Kernel;
using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Commands;

public record AssignWorkOrderCommand(Guid WorkOrderId, AssignWorkOrderDto Dto, string OperatorUsername) : IRequest<Result<WorkOrderItemDto>>;

public class AssignWorkOrderCommandHandler : IRequestHandler<AssignWorkOrderCommand, Result<WorkOrderItemDto>>
{
    private readonly IWorkOrderItemRepository _itemRepository;
    private readonly IWorkOrderTypeRepository _typeRepository;

    public AssignWorkOrderCommandHandler(IWorkOrderItemRepository itemRepository, IWorkOrderTypeRepository typeRepository)
    {
        _itemRepository = itemRepository;
        _typeRepository = typeRepository;
    }

    public async Task<Result<WorkOrderItemDto>> Handle(AssignWorkOrderCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdWithDetailsAsync(request.WorkOrderId, cancellationToken);
        if (item is null)
            return Result.Failure<WorkOrderItemDto>("İş emri bulunamadı");

        var result = item.Assign(request.Dto.AssignedTo, request.Dto.AssignedTeam, request.OperatorUsername);
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