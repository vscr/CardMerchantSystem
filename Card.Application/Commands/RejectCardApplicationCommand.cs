using Card.Domain.Enums;
using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Başvuru reddetme komutu
/// </summary>
public record RejectCardApplicationCommand(
    Guid ApplicationId,
    string Reason,
    string RejectorUsername
) : IRequest<Result>;
public class RejectCardApplicationCommandHandler
    : IRequestHandler<RejectCardApplicationCommand, Result>
{
    private readonly ICardApplicationRepository _repository;

    public RejectCardApplicationCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        RejectCardApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Başvuruyu bul
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);

        if (application == null)
            return Result.Failure("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        // 2. Önce incelemeye alınmış olmalı
        if (application.Status != CardApplicationStatus.UnderReview)
        {
            if (application.Status == CardApplicationStatus.Pending)
            {
                var reviewResult = application.StartReview(request.RejectorUsername);
                if (reviewResult.IsFailure)
                    return reviewResult;
            }
            else
            {
                return Result.Failure(
                    $"Başvuru reddedilemez. Mevcut durum: {application.Status.DisplayName}",
                    ErrorCodes.CardApplicationInvalidStatus);
            }
        }

        // 3. Reddet
        var rejectResult = application.Reject(request.Reason, request.RejectorUsername);
        if (rejectResult.IsFailure)
            return rejectResult;

        // 4. Kaydet
        await _repository.UpdateAsync(application, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}