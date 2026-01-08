using Merchant.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Commands;

public class ActivateTerminalCommandHandler
    : IRequestHandler<ActivateTerminalCommand, Result>
{
    private readonly IMerchantRepository _repository;

    public ActivateTerminalCommandHandler(IMerchantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        ActivateTerminalCommand request,
        CancellationToken cancellationToken)
    {
        var merchant = await _repository.GetByIdWithTerminalsAsync(request.MerchantId, cancellationToken);

        if (merchant == null)
            return Result.Failure("Üye işyeri bulunamadı", ErrorCodes.MerchantNotFound);

        var result = merchant.ActivateTerminal(request.TerminalId, request.OperatorUsername);
        if (result.IsFailure)
            return result;

        await _repository.UpdateAsync(merchant, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}