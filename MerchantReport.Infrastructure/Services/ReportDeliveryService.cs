using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using Renci.SshNet;

namespace MerchantReport.Infrastructure.Services;

public class ReportDeliveryService : IReportDeliveryService
{
    public async Task<Result<string>> SendViaEmailAsync(
        ReportRequest request,
        MerchantReportConfig config,
        byte[] fileContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(config.EmailRecipients))
                return Result.Failure<string>("E-posta alıcı listesi boş");

            var recipients = config.EmailRecipients.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Simülasyon: Gerçek sistemde SMTP/SendGrid/Mailgun kullanılır
            await Task.Delay(100, cancellationToken);

            var details = $"E-posta gönderildi: {string.Join(", ", recipients)}";
            return Result.Success(details);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"E-posta gönderme hatası: {ex.Message}");
        }
    }

    public async Task<Result<string>> SendViaFtpAsync(
        ReportRequest request,
        MerchantReportConfig config,
        byte[] fileContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(config.FtpHost))
                return Result.Failure<string>("FTP sunucu adresi boş");

            if (config.UseSftp)
            {
                return await SendViaSftpInternalAsync(request, config, fileContent, cancellationToken);
            }

            // FTP simülasyonu
            await Task.Delay(100, cancellationToken);

            var remotePath = Path.Combine(config.FtpPath ?? "/", request.FileName ?? "report.pdf");
            var details = $"FTP'ye yüklendi: {config.FtpHost}:{config.FtpPort}{remotePath}";
            return Result.Success(details);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"FTP gönderme hatası: {ex.Message}");
        }
    }

    private async Task<Result<string>> SendViaSftpInternalAsync(
        ReportRequest request,
        MerchantReportConfig config,
        byte[] fileContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // SSH.NET ile SFTP bağlantısı
            using var client = new SftpClient(
                config.FtpHost!,
                config.FtpPort ?? 22,
                config.FtpUsername,
                config.FtpPassword);

            await Task.Run(() => client.Connect(), cancellationToken);

            var remotePath = Path.Combine(config.FtpPath ?? "/", request.FileName ?? "report.pdf");

            using var stream = new MemoryStream(fileContent);
            await Task.Run(() => client.UploadFile(stream, remotePath), cancellationToken);

            client.Disconnect();

            var details = $"SFTP'ye yüklendi: {config.FtpHost}:{config.FtpPort}{remotePath}";
            return Result.Success(details);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"SFTP gönderme hatası: {ex.Message}");
        }
    }

    public async Task<Result<string>> SendViaApiCallbackAsync(
        ReportRequest request,
        MerchantReportConfig config,
        byte[] fileContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(config.CallbackUrl))
                return Result.Failure<string>("Callback URL boş");

            // Simülasyon: Gerçek sistemde HttpClient kullanılır
            using var httpClient = new HttpClient();

            if (!string.IsNullOrEmpty(config.CallbackApiKey))
            {
                httpClient.DefaultRequestHeaders.Add("X-API-Key", config.CallbackApiKey);
            }

            var content = new MultipartFormDataContent
            {
                { new ByteArrayContent(fileContent), "file", request.FileName ?? "report.pdf" },
                { new StringContent(request.RequestNumber), "requestNumber" },
                { new StringContent(request.MerchantId), "merchantId" }
            };

            // Simülasyon
            await Task.Delay(100, cancellationToken);

            var details = $"API Callback gönderildi: {config.CallbackUrl}";
            return Result.Success(details);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>($"API Callback hatası: {ex.Message}");
        }
    }
}