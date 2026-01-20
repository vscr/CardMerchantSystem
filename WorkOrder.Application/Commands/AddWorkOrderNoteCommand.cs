using CardMerchantSystem.Shared.Kernel;
using MediatR;
using WorkOrder.Application.DTOs;
using WorkOrder.Domain.Repositories;

namespace WorkOrder.Application.Commands;

public record AddWorkOrderNoteCommand(Guid WorkOrderId, AddNoteDto Dto, string CreatedBy) : IRequest<Result<WorkOrderNoteDto>>;

public class AddWorkOrderNoteCommandHandler : IRequestHandler<AddWorkOrderNoteCommand, Result<WorkOrderNoteDto>>
{
    private readonly IWorkOrderItemRepository _repository;

    public AddWorkOrderNoteCommandHandler(IWorkOrderItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<WorkOrderNoteDto>> Handle(AddWorkOrderNoteCommand request, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdWithDetailsAsync(request.WorkOrderId, cancellationToken);
        if (item is null)
            return Result.Failure<WorkOrderNoteDto>("İş emri bulunamadı");

        item.AddNote(request.Dto.Content, request.CreatedBy, request.Dto.IsInternal);

        _repository.Update(item);
        await _repository.SaveChangesAsync(cancellationToken);

        var note = item.Notes.Last();
        return new WorkOrderNoteDto
        {
            Id = note.Id,
            WorkOrderItemId = note.WorkOrderItemId,
            Content = note.Content,
            IsInternal = note.IsInternal,
            CreatedAt = note.CreatedAt,
            CreatedBy = note.CreatedBy
        };
    }
}