using CardMerchantSystem.Shared.Kernel;
using Dispute.Domain.Repositories;
using MediatR;

namespace Dispute.Application.Commands;

public record AddDisputeNoteCommand(
    Guid DisputeId,
    string Note,
    string Username,
    bool IsInternal
) : IRequest<Result>;
public class AddDisputeNoteCommandHandler : IRequestHandler<AddDisputeNoteCommand, Result>
{
    private readonly IDisputeRepository _repository;

    public AddDisputeNoteCommandHandler(IDisputeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(AddDisputeNoteCommand request, CancellationToken cancellationToken)
    {
        var dispute = await _repository.GetByIdWithDetailsAsync(request.DisputeId, cancellationToken);

        if (dispute == null)
            return Result.Failure("İtiraz bulunamadı", ErrorCodes.NotFound);

        dispute.AddNote(request.Note, request.Username, request.IsInternal);

        await _repository.UpdateAsync(dispute, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}