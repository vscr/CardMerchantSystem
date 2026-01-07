using Card.Domain.Enums;
using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

public class ApproveCardApplicationCommandHandler
    : IRequestHandler<ApproveCardApplicationCommand, Result>
{
    private readonly ICardApplicationRepository _repository;

    public ApproveCardApplicationCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        ApproveCardApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Başvuruyu bul
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);

        if (application == null)
            return Result.Failure("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        // 2. Önce incelemeye alınmış olmalı
        if (application.Status != CardApplicationStatus.UnderReview)
        {
            // Eğer Pending ise önce incelemeye al
            if (application.Status == CardApplicationStatus.Pending)
            {
                var reviewResult = application.StartReview(request.ApproverUsername);
                if (reviewResult.IsFailure)
                    return reviewResult;
            }
            else
            {
                return Result.Failure(
                    $"Başvuru onaylanamaz. Mevcut durum: {application.Status.DisplayName}",
                    ErrorCodes.CardApplicationInvalidStatus);
            }
        }

        // 3. Onayla
        var approveResult = application.Approve(request.ApproverUsername);
        if (approveResult.IsFailure)
            return approveResult;

        // 4. Kaydet
        await _repository.UpdateAsync(application, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}