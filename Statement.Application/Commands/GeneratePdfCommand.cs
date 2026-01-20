using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Statement.Domain.Repositories;
using Statement.Domain.Services;

namespace Statement.Application.Commands;

public record GeneratePdfCommand(Guid StatementId) : IRequest<Result<byte[]>>;
public class GeneratePdfCommandHandler
    : IRequestHandler<GeneratePdfCommand, Result<byte[]>>
{
    private readonly ICardStatementRepository _repository;
    private readonly IStatementPdfService _pdfService;

    public GeneratePdfCommandHandler(
        ICardStatementRepository repository,
        IStatementPdfService pdfService)
    {
        _repository = repository;
        _pdfService = pdfService;
    }

    public async Task<Result<byte[]>> Handle(
        GeneratePdfCommand request,
        CancellationToken cancellationToken)
    {
        var statement = await _repository.GetByIdWithItemsAsync(request.StatementId, cancellationToken);
        if (statement == null)
            return Result.Failure<byte[]>("Ekstre bulunamadı");

        var pdfResult = await _pdfService.GeneratePdfAsync(statement, cancellationToken);
        if (pdfResult.IsFailure)
            return Result.Failure<byte[]>(pdfResult.Error!);

        // PDF yolunu kaydet
        var saveResult = await _pdfService.SavePdfAsync(statement, pdfResult.Value!, cancellationToken);
        if (saveResult.IsSuccess)
        {
            statement.SetPdfPath(saveResult.Value!);
            await _repository.UpdateAsync(statement, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        return pdfResult.Value!;
    }
}