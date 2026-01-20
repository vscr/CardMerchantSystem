using CardMerchantSystem.Shared.Kernel;

namespace RegulatoryReporting.Domain.Entities;

/// <summary>
/// Rapor gönderimi
/// </summary>
public class ReportSubmission : Entity
{
    public Guid GeneratedReportId { get; private set; }

    // Gönderim bilgileri
    public string SubmissionMethod { get; private set; } = null!; // FTP, API, Email, Portal
    public string? SubmissionReference { get; private set; }

    // Tarihler
    public DateTime SubmittedAt { get; private set; }
    public DateTime? AcknowledgedAt { get; private set; }

    // Gönderen
    public string SubmittedBy { get; private set; } = null!;

    // Durum
    public bool IsSuccessful { get; private set; }
    public string? ResponseMessage { get; private set; }
    public string? ErrorDetails { get; private set; }

    private ReportSubmission() { }

    public static ReportSubmission Create(
        Guid generatedReportId,
        string submissionMethod,
        string submittedBy)
    {
        return new ReportSubmission
        {
            GeneratedReportId = generatedReportId,
            SubmissionMethod = submissionMethod,
            SubmittedBy = submittedBy,
            SubmittedAt = DateTime.UtcNow,
            IsSuccessful = false
        };
    }

    /// <summary>
    /// Gönderim başarılı
    /// </summary>
    public void MarkSuccess(string? reference, string? responseMessage)
    {
        IsSuccessful = true;
        SubmissionReference = reference;
        ResponseMessage = responseMessage;
        AcknowledgedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gönderim başarısız
    /// </summary>
    public void MarkFailure(string errorDetails)
    {
        IsSuccessful = false;
        ErrorDetails = errorDetails;
    }
}