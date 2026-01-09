namespace Dispute.Application.DTOs;

/// <summary>
/// İtiraz detay DTO (dökümanlar ve notlarla birlikte)
/// </summary>
public class DisputeDetailDto
{
    public DisputeDto Dispute { get; set; } = null!;
    public List<DisputeDocumentDto> Documents { get; set; } = new();
    public List<DisputeNoteDto> Notes { get; set; } = new();
}

/// <summary>
/// İtiraz dökümanı DTO
/// </summary>
public class DisputeDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public long FileSize { get; set; }
    public string UploadedBy { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// İtiraz notu DTO
/// </summary>
public class DisputeNoteDto
{
    public Guid Id { get; set; }
    public string Note { get; set; } = null!;
    public string CreatedByUser { get; set; } = null!;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
}