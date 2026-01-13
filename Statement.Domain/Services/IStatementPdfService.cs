using Statement.Domain.Entities;
using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Services;

/// <summary>
/// Ekstre PDF Servisi
/// </summary>
public interface IStatementPdfService
{
    /// <summary>
    /// Ekstre PDF'i oluşturur
    /// </summary>
    Task<Result<byte[]>> GeneratePdfAsync(
        CardStatement statement,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// PDF'i dosyaya kaydeder
    /// </summary>
    Task<Result<string>> SavePdfAsync(
        CardStatement statement,
        byte[] pdfContent,
        CancellationToken cancellationToken = default);
}