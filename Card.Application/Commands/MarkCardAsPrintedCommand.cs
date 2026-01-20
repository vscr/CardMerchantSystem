using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Kart basıldı olarak işaretleme komutu
/// </summary>
public record MarkCardAsPrintedCommand(
    Guid ApplicationId,
    string EncryptedCardNumber,
    string MaskedCardNumber,
    string OperatorUsername
) : IRequest<Result>;
public class MarkCardAsPrintedCommandHandler
    : IRequestHandler<MarkCardAsPrintedCommand, Result>
{
    private readonly ICardApplicationRepository _repository;

    public MarkCardAsPrintedCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        MarkCardAsPrintedCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Başvuruyu bul
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);

        if (application == null)
            return Result.Failure("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        // 2. Kart basıldı olarak işaretle
        var result = application.MarkAsPrinted(
            request.EncryptedCardNumber,
            request.MaskedCardNumber,
            request.OperatorUsername);

        if (result.IsFailure)
            return result;

        // 3. Kaydet
        await _repository.UpdateAsync(application, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}