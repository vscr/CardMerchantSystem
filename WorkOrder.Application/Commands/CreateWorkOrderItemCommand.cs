using CardMerchantSystem.Shared.Kernel;
using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Entities;
using WorkOrder.Domain.Enums;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Commands;

public record CreateWorkOrderItemCommand(CreateWorkOrderItemDto Dto) : IRequest<Result<WorkOrderItemDto>>;

public class CreateWorkOrderItemCommandHandler : IRequestHandler<CreateWorkOrderItemCommand, Result<WorkOrderItemDto>>
{
    private readonly IWorkOrderItemRepository _itemRepository;
    private readonly IWorkOrderTypeRepository _typeRepository;

    public CreateWorkOrderItemCommandHandler(
        IWorkOrderItemRepository itemRepository,
        IWorkOrderTypeRepository typeRepository)
    {
        _itemRepository = itemRepository;
        _typeRepository = typeRepository;
    }

    public async Task<Result<WorkOrderItemDto>> Handle(CreateWorkOrderItemCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var type = await _typeRepository.GetByIdAsync(dto.WorkOrderTypeId, cancellationToken);
        if (type is null)
            return Result.Failure<WorkOrderItemDto>("İş emri tipi bulunamadı");

        if (!type.IsActive)
            return Result.Failure<WorkOrderItemDto>("İş emri tipi aktif değil");

        var priority = Enumeration.FromId<WorkOrderPriority>(dto.PriorityId);
        if (priority is null)
            return Result.Failure<WorkOrderItemDto>("Geçersiz öncelik");

        var itemResult = WorkOrderItem.Create(
            dto.WorkOrderTypeId, type.SlaHours, type.RequiresApproval,
            dto.Subject, dto.Description, priority,
            dto.CardId, dto.CustomerId, dto.CustomerName, dto.CustomerPhone);

        if (itemResult.IsFailure)
            return Result.Failure<WorkOrderItemDto>(itemResult.Error);

        var item = itemResult.Value!;
        await _itemRepository.AddAsync(item, cancellationToken);
        await _itemRepository.SaveChangesAsync(cancellationToken);

        return MapToDto(item, type);
    }

    private static WorkOrderItemDto MapToDto(WorkOrderItem item, WorkOrderType type) => new()
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