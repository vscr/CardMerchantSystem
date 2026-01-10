namespace HSM.Application.DTOs;

/// <summary>
/// HSM Key DTO
/// </summary>
public class HSMKeyDto
{
    public Guid Id { get; set; }
    public string KeyName { get; set; } = null!;
    public string KeyType { get; set; } = null!;
    public string KeyTypeDisplayName { get; set; } = null!;
    public string KeyIndex { get; set; } = null!;
    public string KeyCheckValue { get; set; } = null!;
    public int KeyLength { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Description { get; set; }
    public Guid HSMDeviceId { get; set; }
    public string? HSMDeviceName { get; set; }
    public bool IsExpired { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// HSM Key oluşturma DTO
/// </summary>
public class CreateHSMKeyDto
{
    public string KeyName { get; set; } = null!;
    public int KeyTypeId { get; set; }
    public int KeyLength { get; set; } = 32;
    public DateTime? ExpiryDate { get; set; }
    public string? Description { get; set; }
    public Guid HSMDeviceId { get; set; }
}