using CardMerchantSystem.Shared.Kernel;
using MediatR;
using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Repositories;

namespace BulkCardPrint.Application.Commands;

public record AddItemsToPrintBatchCommand(Guid BatchId, List<AddPrintBatchItemDto> Items) : IRequest<Result<PrintBatchDto>>;

public record AddPrintBatchItemDto(
    Guid CardApplicationId,
    string CustomerName,
    string CustomerSurname,
    string CustomerTckn,
    string CardType,
    string DeliveryAddress);

public class AddItemsToPrintBatchCommandHandler : IRequestHandler<AddItemsToPrintBatchCommand, Result<PrintBatchDto>>
{
    private readonly IPrintBatchRepository _repository;

    public AddItemsToPrintBatchCommandHandler(IPrintBatchRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PrintBatchDto>> Handle(AddItemsToPrintBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = await _repository.GetByIdWithItemsAsync(request.BatchId, cancellationToken);
        if (batch is null)
            return Result.Failure<PrintBatchDto>("Batch bulunamadı");

        var sequenceNumber = batch.Items.Count;

        foreach (var itemDto in request.Items)
        {
            sequenceNumber++;
            var item = PrintBatchItem.Create(
                batch.Id,
                itemDto.CardApplicationId,
                itemDto.CustomerName,
                itemDto.CustomerSurname,
                itemDto.CustomerTckn,
                itemDto.CardType,
                itemDto.DeliveryAddress,
                sequenceNumber);

            var addResult = batch.AddItem(item);
            if (addResult.IsFailure)
                return Result.Failure<PrintBatchDto>(addResult.Error);
        }

        _repository.Update(batch);
        await _repository.SaveChangesAsync(cancellationToken);

        return new PrintBatchDto
        {
            Id = batch.Id,
            BatchNumber = batch.BatchNumber,
            PrintVendorId = batch.PrintVendorId,
            VendorName = batch.Vendor?.Name ?? "",
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