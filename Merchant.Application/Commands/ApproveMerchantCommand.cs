using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Merchant.Domain.Repositories;

namespace Merchant.Application.Commands;

public record ApproveMerchantCommand(
    Guid MerchantId,
    string ApproverUsername
) : IRequest<Result>;
public class ApproveMerchantCommandHandler
    : IRequestHandler<ApproveMerchantCommand, Result>
{
    private readonly IMerchantRepository _repository;

    public ApproveMerchantCommandHandler(IMerchantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        ApproveMerchantCommand request,
        CancellationToken cancellationToken)
    {
        var merchant = await _repository.GetByIdAsync(request.MerchantId, cancellationToken);

        if (merchant == null)
            return Result.Failure("Üye işyeri bulunamadı", ErrorCodes.MerchantNotFound);

        var result = merchant.Approve(request.ApproverUsername);
        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(merchant, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}