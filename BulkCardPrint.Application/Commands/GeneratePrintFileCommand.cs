using CardMerchantSystem.Shared.Kernel;
using MediatR;
using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace BulkCardPrint.Application.Commands;

public record GeneratePrintFileCommand(Guid BatchId, string OperatorUsername) : IRequest<Result<PrintBatchDto>>;

public class GeneratePrintFileCommandHandler : IRequestHandler<GeneratePrintFileCommand, Result<PrintBatchDto>>
{
    private readonly IPrintBatchRepository _batchRepository;
    private readonly IPrintVendorRepository _vendorRepository;

    public GeneratePrintFileCommandHandler(
        IPrintBatchRepository batchRepository,
        IPrintVendorRepository vendorRepository)
    {
        _batchRepository = batchRepository;
        _vendorRepository = vendorRepository;
    }

    public async Task<Result<PrintBatchDto>> Handle(GeneratePrintFileCommand request, CancellationToken cancellationToken)
    {
        var batch = await _batchRepository.GetByIdWithItemsAsync(request.BatchId, cancellationToken);
        if (batch is null)
            return Result.Failure<PrintBatchDto>("Batch bulunamadı");

        var vendor = await _vendorRepository.GetByIdAsync(batch.PrintVendorId, cancellationToken);
        if (vendor is null)
            return Result.Failure<PrintBatchDto>("Basım firması bulunamadı");

        // Dosya içeriği oluştur (CSV formatında örnek)
        var fileContent = GenerateFileContent(batch);
        var checksum = ComputeChecksum(fileContent);

        var fileName = $"{batch.BatchNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        var filePath = $"/exports/print-batches/{fileName}";

        // Gerçek senaryoda dosya diske yazılacak
        // File.WriteAllText(filePath, fileContent);

        var markResult = batch.MarkFileGenerated(fileName, filePath, checksum, request.OperatorUsername);
        if (markResult.IsFailure)
            return Result.Failure<PrintBatchDto>(markResult.Error);

        _batchRepository.Update(batch);
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

    private static string GenerateFileContent(Domain.Entities.PrintBatch batch)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SequenceNumber,CustomerName,CustomerSurname,TCKN,CardType,DeliveryAddress");

        foreach (var item in batch.Items.OrderBy(i => i.SequenceNumber))
        {
            sb.AppendLine($"{item.SequenceNumber},{item.CustomerName},{item.CustomerSurname},{item.CustomerTckn},{item.CardType},\"{item.DeliveryAddress}\"");
        }

        return sb.ToString();
    }

    private static string ComputeChecksum(string content)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}