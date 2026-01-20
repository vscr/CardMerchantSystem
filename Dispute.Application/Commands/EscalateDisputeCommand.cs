using CardMerchantSystem.Shared.Kernel;
using Dispute.Domain.Repositories;
using MediatR;

namespace Dispute.Application.Commands;

public record EscalateDisputeCommand(
    Guid DisputeId,
    string EscalationReason,
    string OperatorUsername
) : IRequest<Result>;
public class EscalateDisputeCommandHandler : IRequestHandler<EscalateDisputeCommand, Result>
{
    private readonly IDisputeRepository _repository;

    public EscalateDisputeCommandHandler(IDisputeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(EscalateDisputeCommand request, CancellationToken cancellationToken)
    {
        var dispute = await _repository.GetByIdAsync(request.DisputeId, cancellationToken);

        if (dispute == null)
            return Result.Failure("İtiraz bulunamadı", ErrorCodes.NotFound);

        var result = dispute.EscalateToBank(request.EscalationReason, request.OperatorUsername);

        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(dispute, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}