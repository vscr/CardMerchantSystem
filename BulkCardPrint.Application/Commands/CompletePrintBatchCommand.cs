using CardMerchantSystem.Shared.Kernel;
using MediatR;
using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Repositories;

namespace BulkCardPrint.Application.Commands;

public record CompletePrintBatchCommand(Guid BatchId, string OperatorUsername) : IRequest<Result<PrintBatchDto>>;

public class CompletePrintBatchCommandHandler : IRequestHandler<CompletePrintBatchCommand, Result<PrintBatchDto>>
{
    private readonly IPrintBatchRepository _batchRepository;
    private readonly IPrintVendorRepository _vendorRepository;

    public CompletePrintBatchCommandHandler(
        IPrintBatchRepository batchRepository,
        IPrintVendorRepository vendorRepository)
    {
        _batchRepository = batchRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<PrintBatchDto>> Handle(CompletePrintBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdWithItemsAsync(request.BatchId, cancellationToken);
        if (batch is null)
            return Result.Failure<PrintBatchDto>("Batch bulunamadı");

        var vendor = await _vendorRepository.GetByIdAsync(batch.PrintVendorId, cancellationToken);

        var completeResult = batch.Complete(request.OperatorUsername);
        if (completeResult.IsFailure)
            return Result.Failure<PrintBatchDto>(completeResult.Error);

        _batchRepository.Update(batch);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        return new PrintBatchDto
        {
            Id = batch.Id,
            BatchNumber = batch.BatchNumber,
            PrintVendorId = batch.PrintVendorId,
            VendorName = vendor?.Name ?? "",
            Status = batch.Status.Name,
            StatusDisplayName = batch.Status.DisplayName,
            TotalItemCount = batch.TotalItemCount,
            PrintedCount = batch.PrintedCount,
            FailedCount = batch.FailedCount,
            FileName = batch.FileName,
            FilePath = batch.FilePath,
            FileGeneratedAt = batch.FileGeneratedAt,
            SentToVendorAt = batch.SentToVendorAt,
            ProductionStartedAt = batch.ProductionStartedAt,
            CompletedAt = batch.CompletedAt,
            Notes = batch.Notes,
            FailureReason = batch.FailureReason,
            CreatedAt = batch.CreatedAt
        };
    }
}