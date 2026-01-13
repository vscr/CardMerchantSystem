using Statement.Domain.Entities;
using Statement.Domain.Enums;
using Statement.Domain.Repositories;
using Statement.Domain.Services;
using CardMerchantSystem.Shared.Kernel;

namespace Statement.Infrastructure.Services;

public class StatementNotificationService : IStatementNotificationService
{
    private readonly IStatementNotificationRepository _notificationRepository;
    private readonly ICardStatementRepository _statementRepository;

    public StatementNotificationService(
        IStatementNotificationRepository notificationRepository,
        ICardStatementRepository statementRepository)
    {
        _notificationRepository = notificationRepository;
        _statementRepository = statementRepository;
    }

    public async Task<Result> SendNotificationAsync(
        CardStatement statement,
        NotificationType notificationType,
        CancellationToken cancellationToken = default)
    {
        string recipient;
        string? subject = null;
        string? content = null;

        switch (notificationType.Name)
        {
            case nameof(NotificationType.Email):
                if (string.IsNullOrEmpty(statement.CustomerEmail))
                    return Result.Failure("E-posta adresi bulunamadı");

                recipient = statement.CustomerEmail;
                subject = $"Kart Ekstreniz Hazır - {statement.StatementNumber}";
                content = GenerateEmailContent(statement);
                break;

            case nameof(NotificationType.SMS):
                if (string.IsNullOrEmpty(statement.CustomerPhone))
                    return Result.Failure("Telefon numarası bulunamadı");

                recipient = statement.CustomerPhone;
                content = GenerateSmsContent(statement);
                break;

            default:
                return Result.Failure("Desteklenmeyen bildirim tipi");
        }

        var notification = StatementNotification.Create(
            statement.Id,
            notificationType,
            recipient,
            subject,
            content);

        // Simülasyon: Gerçek entegrasyonda burada mail/SMS gönderimi yapılır
        try
        {
            // Email veya SMS gönderimi simülasyonu
            await SimulateSendAsync(notificationType, recipient, subject, content, cancellationToken);

            notification.MarkAsSent();
        }
        catch (Exception ex)
        {
            notification.RecordError(ex.Message);
        }

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        if (notification.IsSent)
        {
            statement.MarkAsSent();
            await _statementRepository.UpdateAsync(statement, cancellationToken);
            await _statementRepository.SaveChangesAsync(cancellationToken);
        }

        return notification.IsSent
            ? Result.Success()
            : Result.Failure($"Bildirim gönderilemedi: {notification.ErrorMessage}");
    }

    public async Task<Result> SendPaymentReminderAsync(
        CardStatement statement,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(statement.CustomerPhone))
            return Result.Failure("Telefon numarası bulunamadı");

        var content = $"Sayin {statement.CustomerName}, {statement.MaskedCardNumber} numarali kartinizin " +
                      $"{statement.DueDate:dd.MM.yyyy} son odeme tarihli {statement.RemainingBalance:N2} TL borcunuz bulunmaktadir.";

        var notification = StatementNotification.Create(
            statement.Id,
            NotificationType.SMS,
            statement.CustomerPhone,
            null,
            content);

        try
        {
            await SimulateSendAsync(NotificationType.SMS, statement.CustomerPhone, null, content, cancellationToken);
            notification.MarkAsSent();
        }
        catch (Exception ex)
        {
            notification.RecordError(ex.Message);
        }

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return notification.IsSent ? Result.Success() : Result.Failure(notification.ErrorMessage!);
    }

    public async Task<Result> SendOverdueNotificationAsync(
        CardStatement statement,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(statement.CustomerPhone))
            return Result.Failure("Telefon numarası bulunamadı");

        var daysOverdue = (int)(DateTime.UtcNow.Date - statement.DueDate).TotalDays;

        var content = $"Sayin {statement.CustomerName}, {statement.MaskedCardNumber} numarali kartinizin " +
                      $"{statement.RemainingBalance:N2} TL tutarindaki borcunuz {daysOverdue} gundur gecikmiştir. " +
                      $"Lutfen en kisa surede odeme yapiniz.";

        var notification = StatementNotification.Create(
            statement.Id,
            NotificationType.SMS,
            statement.CustomerPhone,
            null,
            content);

        try
        {
            await SimulateSendAsync(NotificationType.SMS, statement.CustomerPhone, null, content, cancellationToken);
            notification.MarkAsSent();
        }
        catch (Exception ex)
        {
            notification.RecordError(ex.Message);
        }

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return notification.IsSent ? Result.Success() : Result.Failure(notification.ErrorMessage!);
    }

    private static string GenerateEmailContent(CardStatement statement)
    {
        return $@"
            Sayın {statement.CustomerName},
            
            {statement.MaskedCardNumber} numaralı kartınıza ait {statement.PeriodEndDate:MMMM yyyy} dönemi ekstreniz hazırlanmıştır.
            
            Dönem Sonu Bakiye: {statement.CurrentBalance:N2} TL
            Minimum Ödeme: {statement.MinimumPayment:N2} TL
            Son Ödeme Tarihi: {statement.DueDate:dd.MM.yyyy}
            
            Ekstre detaylarınızı internet bankacılığı veya mobil uygulamamızdan görüntüleyebilirsiniz.
            
            Saygılarımızla,
            Card Merchant Bank";
    }

    private static string GenerateSmsContent(CardStatement statement)
    {
        return $"Sayin {statement.CustomerName}, {statement.MaskedCardNumber} kartinizin " +
               $"{statement.CurrentBalance:N2} TL borcunun son odeme tarihi {statement.DueDate:dd.MM.yyyy}'dir.";
    }

    private static Task SimulateSendAsync(
        NotificationType type,
        string recipient,
        string? subject,
        string? content,
        CancellationToken cancellationToken)
    {
        // Gerçek uygulamada:
        // - Email için SMTP veya SendGrid/Mailgun entegrasyonu
        // - SMS için Twilio/Netgsm entegrasyonu
        // Şimdilik simülasyon - her zaman başarılı
        return Task.Delay(100, cancellationToken);
    }
}