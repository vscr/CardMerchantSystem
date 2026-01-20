using CardMerchantSystem.Shared.Kernel;
using Dispute.Domain.Repositories;
using MediatR;

namespace Dispute.Application.Commands;

public record StartReviewCommand(Guid DisputeId, string AssignedTo) : IRequest<Result>;
public class StartReviewCommandHandler : IRequestHandler<StartReviewCommand, Result>
{
    private readonly IDisputeRepository _repository;

    public StartReviewCommandHandler(IDisputeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(StartReviewCommand request, CancellationToken cancellationToken)
    {
        var dispute = await _repository.GetByIdAsync(request.DisputeId, cancellationToken);

        if (dispute == null)
            return Result.Failure("İtiraz bulunamadı", ErrorCodes.NotFound);

        var result = dispute.StartReview(request.AssignedTo);

        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(dispute, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}