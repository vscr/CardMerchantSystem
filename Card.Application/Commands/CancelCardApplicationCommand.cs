using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Başvuru iptal komutu
/// </summary>
public record CancelCardApplicationCommand(
    Guid ApplicationId,
    string Reason,
    string OperatorUsername
) : IRequest<Result>;
public class CancelCardApplicationCommandHandler
    : IRequestHandler<CancelCardApplicationCommand, Result>
{
    private readonly ICardApplicationRepository _repository;

    public CancelCardApplicationCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        CancelCardApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Başvuruyu bul
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);

        if (application == null)
            return Result.Failure("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        // 2. İptal et
        var result = application.Cancel(request.Reason, request.OperatorUsername);

        if (result.IsFailure)
            return result;

        // 3. Kaydet
        await _repository.UpdateAsync(application, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}