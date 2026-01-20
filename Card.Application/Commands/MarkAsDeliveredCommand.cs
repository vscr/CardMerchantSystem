using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Kart teslim edildi komutu
/// </summary>
public record MarkAsDeliveredCommand(
    Guid ApplicationId,
    string OperatorUsername
) : IRequest<Result>;
public class MarkAsDeliveredCommandHandler
    : IRequestHandler<MarkAsDeliveredCommand, Result>
{
    private readonly ICardApplicationRepository _repository;

    public MarkAsDeliveredCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        MarkAsDeliveredCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Başvuruyu bul
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);

        if (application == null)
            return Result.Failure("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        // 2. Teslim edildi olarak işaretle
        var result = application.MarkAsDelivered(request.OperatorUsername);

        if (result.IsFailure)
            return result;

        // 3. Kaydet
        await _repository.UpdateAsync(application, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}