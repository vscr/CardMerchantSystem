using MerchantReport.Domain.Entities;
using CardMerchantSystem.Shared.Kernel;

namespace MerchantReport.Domain.Services;

/// <summary>
/// Rapor Dağıtım Servisi
/// </summary>
public interface IReportDeliveryService
{
    /// <summary>
    /// Email ile gönderir
    /// </summary>
    Task<Result<string>> SendViaEmailAsync(
        ReportRequest request,
        MerchantReportConfig config,
        byte[] fileContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// FTP/SFTP ile gönderir
    /// </summary>
    Task<Result<string>> SendViaFtpAsync(
        ReportRequest request,
        MerchantReportConfig config,
        byte[] fileContent,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// API Callback ile gönderir
    /// </summary>
    Task<Result<string>> SendViaApiCallbackAsync(
        ReportRequest request,
        MerchantReportConfig config,
        byte[] fileContent,
        CancellationToken cancellationToken = default);
}