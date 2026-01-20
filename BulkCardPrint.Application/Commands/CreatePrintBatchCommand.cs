using CardMerchantSystem.Shared.Kernel;
using MediatR;
using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Repositories;

namespace BulkCardPrint.Application.Commands;

public record CreatePrintBatchCommand(CreatePrintBatchDto Dto) : IRequest<Result<PrintBatchDto>>;

public class CreatePrintBatchCommandHandler : IRequestHandler<CreatePrintBatchCommand, Result<PrintBatchDto>>
{
    private readonly IPrintBatchRepository _batchRepository;
    private readonly IPrintVendorRepository _vendorRepository;

    public CreatePrintBatchCommandHandler(
        IPrintBatchRepository batchRepository,
        IPrintVendorRepository vendorRepository)
    {
        _batchRepository = batchRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<PrintBatchDto>> Handle(CreatePrintBatchCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var vendor = await _vendorRepository.GetByIdAsync(dto.PrintVendorId, cancellationToken);
        if (vendor is null)
            return Result.Failure<PrintBatchDto>("Basım firması bulunamadı");

        if (!vendor.IsActive)
            return Result.Failure<PrintBatchDto>("Basım firması aktif değil");

        // Kapasite kontrolü
        var capacityResult = vendor.AddToLoad(dto.CardApplicationIds.Count);
        if (capacityResult.IsFailure)
            return Result.Failure<PrintBatchDto>(capacityResult.Error);

        var batchResult = PrintBatch.Create(dto.PrintVendorId);
        if (batchResult.IsFailure)
            return Result.Failure<PrintBatchDto>(batchResult.Error);

        var batch = batchResult.Value!;

        // NOT: Gerçek senaryoda CardApplication'lardan item oluşturulacak
        // Şimdilik sadece batch oluşturuyoruz
        // Item'lar ayrı bir endpoint ile eklenebilir

        await _batchRepository.AddAsync(batch, cancellationToken);
        _vendorRepository.Update(vendor);
        await _batchRepository.SaveChangesAsync(cancellationToken);

        return new PrintBatchDto
        {
            Id = batch.Id,
            BatchNumber = batch.BatchNumber,
            PrintVendorId = batch.PrintVendorId,
            VendorName = vendor.Name,
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