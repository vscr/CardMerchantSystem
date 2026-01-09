using Dispute.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Dispute.Application.Commands;

public class ResolveDisputeCommandHandler : IRequestHandler<ResolveDisputeCommand, Result>
{
    private readonly IDisputeRepository _repository;

    public ResolveDisputeCommandHandler(IDisputeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(ResolveDisputeCommand request, CancellationToken cancellationToken)
    {
        var dispute = await _repository.GetByIdAsync(request.DisputeId, cancellationToken);

        if (dispute == null)
            return Result.Failure("İtiraz bulunamadı", ErrorCodes.NotFound);

        Result result;

        if (request.InFavorOfCustomer)
        {
            if (!request.RefundAmount.HasValue)
                return Result.Failure("Müşteri lehine çözüm için iade tutarı belirtilmeli");

            result = dispute.ResolveInFavorOfCustomer(
                request.RefundAmount.Value,
                request.Resolution,
                request.OperatorUsername);
        }
        else
        {
            result = dispute.ResolveInFavorOfMerchant(
                request.Resolution,
                request.OperatorUsername);
        }

        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(dispute, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}