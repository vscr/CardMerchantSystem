using CardMerchantSystem.Shared.Kernel;

namespace Dispute.Domain.Entities;

/// <summary>
/// İtiraz belgesi
/// </summary>
public class DisputeDocument : Entity
{
    public Guid DisputeId { get; private set; }
    public string FileName { get; private set; } = null!;
    public string FileType { get; private set; } = null!;
    public string FilePath { get; private set; } = null!;
    public long FileSize { get; private set; }
    public string UploadedBy { get; private set; } = null!;
    public string? Description { get; private set; }

    // EF Core için
    private DisputeDocument() { }

    public static DisputeDocument Create(
        Guid disputeId,
        string fileName,
        string fileType,
        string filePath,
        long fileSize,
        string uploadedBy,
        string? description = null)
    {
        return new DisputeDocument
        {
            DisputeId = disputeId,
            FileName = fileName,
            FileType = fileType,
            FilePath = filePath,
            FileSize = fileSize,
            UploadedBy = uploadedBy,
            Description = description
        };
    }
}