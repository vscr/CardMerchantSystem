using Dispute.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Commands;

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