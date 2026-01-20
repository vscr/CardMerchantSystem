using Card.Domain.Enums;
using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Kart basım talebi komutu
/// </summary>
public record RequestCardPrintCommand(
    Guid ApplicationId,
    int PrintVendorId,
    string BatchId,
    string OperatorUsername
) : IRequest<Result>;
public class RequestCardPrintCommandHandler
    : IRequestHandler<RequestCardPrintCommand, Result>
{
    private readonly ICardApplicationRepository _repository;

    public RequestCardPrintCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        RequestCardPrintCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Başvuruyu bul
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);

        if (application == null)
            return Result.Failure("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        // 2. Print vendor'ı bul
        var printVendor = PrintVendor.FromId<PrintVendor>(request.PrintVendorId);

        if (printVendor == null)
            return Result.Failure("Geçersiz basım firması", ErrorCodes.ValidationError);

        // 3. Basım talebi oluştur
        var result = application.RequestCardPrint(
            printVendor,
            request.BatchId,
            request.OperatorUsername);

        if (result.IsFailure)
            return result;

        // 4. Kaydet
        await _repository.UpdateAsync(application, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}