using Card.Domain.Enums;
using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

public class StartDeliveryCommandHandler
    : IRequestHandler<StartDeliveryCommand, Result>
{
    private readonly ICardApplicationRepository _repository;

    public StartDeliveryCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        StartDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Başvuruyu bul
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);

        if (application == null)
            return Result.Failure("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        // 2. Önce ReadyForDelivery durumuna geçir (eğer CardPrinted ise)
        if (application.Status == CardApplicationStatus.CardPrinted)
        {
            var readyResult = application.MarkAsReadyForDelivery(request.OperatorUsername);
            if (readyResult.IsFailure)
                return readyResult;
        }

        // 3. Teslimatı başlat
        var result = application.StartDelivery(
            request.TrackingNumber,
            request.OperatorUsername);

        if (result.IsFailure)
            return result;

        // 4. Kaydet
        await _repository.UpdateAsync(application, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}