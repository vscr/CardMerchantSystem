using CardMerchantSystem.Shared.Kernel;
using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Entities;
using WorkOrder.Domain.Enums;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Commands;

public record CreateWorkOrderTypeCommand(CreateWorkOrderTypeDto Dto) : IRequest<Result<WorkOrderTypeDto>>;

public class CreateWorkOrderTypeCommandHandler : IRequestHandler<CreateWorkOrderTypeCommand, Result<WorkOrderTypeDto>>
{
    private readonly IWorkOrderTypeRepository _repository;

    public CreateWorkOrderTypeCommandHandler(IWorkOrderTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<WorkOrderTypeDto>> Handle(CreateWorkOrderTypeCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var existing = await _repository.GetByCodeAsync(dto.Code, cancellationToken);
        if (existing is not null)
            return Result.Failure<WorkOrderTypeDto>("Bu kodla tip zaten mevcut");

        var category = Enumeration.FromId<WorkOrderCategory>(dto.CategoryId);
        if (category is null)
            return Result.Failure<WorkOrderTypeDto>("Geçersiz kategori");

        var typeResult = WorkOrderType.Create(
            dto.Code, dto.Name, dto.Description,
            category, dto.SlaHours, dto.RequiresApproval, dto.ApprovalLevels);

        if (typeResult.IsFailure)
            return Result.Failure<WorkOrderTypeDto>(typeResult.Error);

        var type = typeResult.Value!;
        await _repository.AddAsync(type, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(type);
    }

    private static WorkOrderTypeDto MapToDto(WorkOrderType type) => new()
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