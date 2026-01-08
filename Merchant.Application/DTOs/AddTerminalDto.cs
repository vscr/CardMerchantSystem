namespace Merchant.Application.DTOs;

/// <summary>
/// Terminal ekleme request DTO
/// </summary>
public class AddTerminalDto
{
    public int TerminalTypeId { get; set; }
    public string? SerialNumber { get; set; }
    public string? Model { get; set; }
    public string? Location { get; set; }
}