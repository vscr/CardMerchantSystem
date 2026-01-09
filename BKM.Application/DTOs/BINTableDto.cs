namespace BKM.Application.DTOs;

/// <summary>
/// BIN Table DTO
/// </summary>
public class BINTableDto
{
    public Guid Id { get; set; }
    public string BIN { get; set; } = null!;
    public string BankCode { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public string CardBrand { get; set; } = null!;
    public string CardType { get; set; } = null!;
    public string CardLevel { get; set; } = null!;
    public bool IsActive { get; set; }
}

/// <summary>
/// Create BIN DTO
/// </summary>
public class CreateBINDto
{
    public string BIN { get; set; } = null!;
    public string BankCode { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public string CardBrand { get; set; } = null!;
    public string CardType { get; set; } = null!;
    public string CardLevel { get; set; } = null!;
}