using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Services;

/// <summary>
/// Rapor Oluşturma Servisi
/// </summary>
public interface IReportGeneratorService
{
    /// <summary>
    /// Rapor oluşturur
    /// </summary>
    Task<Result<ReportGenerationResult>> GenerateReportAsync(
        ReportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Üye işyeri ekstresi oluşturur
    /// </summary>
    Task<Result<MerchantStatement>> GenerateMerchantStatementAsync(
        string merchantId,
        DateTime periodStart,
        DateTime periodEnd,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Rapor oluşturma sonucu
/// </summary>
public class ReportGenerationResult
{
    public string FileName { get; set; } = null!;
    public string FilePath { get; set; } = null!;
    public byte[] FileContent { get; set; } = null!;
    public long FileSize { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal NetAmount { get; set; }
}