using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Merchant.Domain.Repositories;

namespace Merchant.Application.Commands;

public record ActivateMerchantCommand(
    Guid MerchantId,
    string OperatorUsername
) : IRequest<Result>;
public class ActivateMerchantCommandHandler
    : IRequestHandler<ActivateMerchantCommand, Result>
{
    private readonly IMerchantRepository _repository;

    public ActivateMerchantCommandHandler(IMerchantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        ActivateMerchantCommand request,
        CancellationToken cancellationToken)
    {
        var merchant = await _repository.GetByIdAsync(request.MerchantId, cancellationToken);

        if (merchant == null)
            return Result.Failure("Üye işyeri bulunamadı", ErrorCodes.MerchantNotFound);

        var result = merchant.Activate(request.OperatorUsername);
        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(merchant, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}