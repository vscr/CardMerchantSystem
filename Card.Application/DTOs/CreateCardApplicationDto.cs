namespace Card.Application.DTOs;

/// <summary>
/// Kart başvurusu oluşturma request DTO
/// </summary>
public class CreateCardApplicationDto
{
    public string CustomerTckn { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public string CustomerSurname { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;

    // Adres
    public string Street { get; set; } = null!;
    public string District { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string? BuildingNo { get; set; }
    public string? ApartmentNo { get; set; }

    // Kart Bilgileri
    public int CardTypeId { get; set; }

    // Opsiyonel limitler (boş ise kart tipine göre default atanır)
    public decimal? DailyLimit { get; set; }
    public decimal? MonthlyLimit { get; set; }
}