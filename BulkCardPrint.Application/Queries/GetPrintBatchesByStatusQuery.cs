using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Enums;
using BulkCardPrint.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BulkCardPrint.Application.Queries;

public record GetPrintBatchesByStatusQuery(int StatusId) : IRequest<IReadOnlyList<PrintBatchDto>>;

public class GetPrintBatchesByStatusQueryHandler : IRequestHandler<GetPrintBatchesByStatusQuery, IReadOnlyList<PrintBatchDto>>
{
    private readonly IPrintBatchRepository _batchRepository;
    private readonly IPrintVendorRepository _vendorRepository;

    public GetPrintBatchesByStatusQueryHandler(
        IPrintBatchRepository batchRepository,
        IPrintVendorRepository vendorRepository)
    {
        _batchRepository = batchRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<IReadOnlyList<PrintBatchDto>> Handle(GetPrintBatchesByStatusQuery request, CancellationToken cancellationToken)
    {
        var status = Enumeration.FromId<PrintBatchStatus>(request.StatusId);
        if (status is null)
            return new List<PrintBatchDto>();

        var batches = await _batchRepository.GetByStatusAsync(status, cancellationToken);
        var vendors = await _vendorRepository.GetAllAsync(cancellationToken);
        var vendorDict = vendors.ToDictionary(v => v.Id, v => v.Name);

        return batches.Select(b => MapToDto(b, vendorDict.GetValueOrDefault(b.PrintVendorId, ""))).ToList();
    }

    private static PrintBatchDto MapToDto(PrintBatch batch, string vendorName)
    {
        return new PrintBatchDto
        {
            Id = batch.Id,
            BatchNumber = batch.BatchNumber,
            PrintVendorId = batch.PrintVendorId,
            VendorName = vendorName,
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