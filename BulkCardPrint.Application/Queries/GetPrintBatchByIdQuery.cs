using MediatR;
using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Repositories;

namespace BulkCardPrint.Application.Queries;

public record GetPrintBatchByIdQuery(Guid Id, bool IncludeItems = false) : IRequest<PrintBatchDto?>;

public class GetPrintBatchByIdQueryHandler : IRequestHandler<GetPrintBatchByIdQuery, PrintBatchDto?>
{
    private readonly IPrintBatchRepository _batchRepository;
    private readonly IPrintVendorRepository _vendorRepository;

    public GetPrintBatchByIdQueryHandler(
        IPrintBatchRepository batchRepository,
        IPrintVendorRepository vendorRepository)
    {
        _batchRepository = batchRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<PrintBatchDto?> Handle(GetPrintBatchByIdQuery request, CancellationToken cancellationToken)
    {
        var batch = request.IncludeItems
            ? await _batchRepository.GetByIdWithItemsAsync(request.Id, cancellationToken)
            : await _batchRepository.GetByIdAsync(request.Id, cancellationToken);

        if (batch is null)
            return null;

        var vendor = await _vendorRepository.GetByIdAsync(batch.PrintVendorId, cancellationToken);

        if (request.IncludeItems)
            return MapToDtoWithItems(batch, vendor?.Name ?? "");

        return MapToDto(batch, vendor?.Name ?? "");
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

    private static PrintBatchWithItemsDto MapToDtoWithItems(PrintBatch batch, string vendorName)
    {
        return new PrintBatchWithItemsDto
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
            CreatedAt = batch.CreatedAt,
            Items = batch.Items.Select(i => new PrintBatchItemDto
            {
                Id = i.Id,
                PrintBatchId = i.PrintBatchId,
                CardApplicationId = i.CardApplicationId,
                CustomerFullName = i.CustomerFullName,
                CustomerTckn = i.CustomerTckn,
                CardType = i.CardType,
                CardNumberMasked = i.CardNumberMasked,
                ExpiryDate = i.ExpiryDate,
                DeliveryAddress = i.DeliveryAddress,
                Status = i.Status.Name,
                StatusDisplayName = i.Status.DisplayName,
                PrintedAt = i.PrintedAt,
                QualityCheckedAt = i.QualityCheckedAt,
                FailureReason = i.FailureReason,
                SequenceNumber = i.SequenceNumber
            }).ToList()
        };
    }
}