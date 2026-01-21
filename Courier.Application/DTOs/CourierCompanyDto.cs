namespace Courier.Application.DTOs;

public class CourierCompanyDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CompanyType { get; set; } = null!;
    public string CompanyTypeDisplayName { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public string? ApiEndpoint { get; set; }
    public string? TrackingUrlTemplate { get; set; }
    public bool IsActive { get; set; }
    public int StandardDeliveryDays { get; set; }
    public int ExpressDeliveryDays { get; set; }
    public decimal BasePrice { get; set; }
    public decimal PricePerKg { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCourierCompanyDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int CompanyTypeId { get; set; }
    public string ContactPerson { get; set; } = null!;
    public string ContactPhone { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public int StandardDeliveryDays { get; set; }
    public decimal BasePrice { get; set; }
    public string? ApiEndpoint { get; set; }
    public string? ApiKey { get; set; }
    public string? TrackingUrlTemplate { get; set; }
}