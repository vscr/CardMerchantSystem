using CardMerchantSystem.Shared.Kernel;

namespace Card.Domain.ValueObjects;

/// <summary>
/// Adres Value Object.
/// Kart teslimat adresi için kullanılır.
/// </summary>
public class Address : ValueObject
{
    public string Street { get; }
    public string District { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }
    public string? BuildingNo { get; }
    public string? ApartmentNo { get; }

    private Address(string street, string district, string city,
        string postalCode, string country, string? buildingNo, string? apartmentNo)
    {
        Street = street;
        District = district;
        City = city;
        PostalCode = postalCode;
        Country = country;
        BuildingNo = buildingNo;
        ApartmentNo = apartmentNo;
    }

    public static Result<Address> Create(
        string street,
        string district,
        string city,
        string postalCode,
        string country = "Türkiye",
        string? buildingNo = null,
        string? apartmentNo = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            return Result.Failure<Address>("Sokak/Cadde boş olamaz", ErrorCodes.ValidationError);

        if (string.IsNullOrWhiteSpace(district))
            return Result.Failure<Address>("İlçe boş olamaz", ErrorCodes.ValidationError);

        if (string.IsNullOrWhiteSpace(city))
            return Result.Failure<Address>("Şehir boş olamaz", ErrorCodes.ValidationError);

        if (string.IsNullOrWhiteSpace(postalCode))
            return Result.Failure<Address>("Posta kodu boş olamaz", ErrorCodes.ValidationError);

        return new Address(
            street.Trim(),
            district.Trim(),
            city.Trim(),
            postalCode.Trim(),
            country.Trim(),
            buildingNo?.Trim(),
            apartmentNo?.Trim());
    }

    /// <summary>
    /// Tek satır formatında adres
    /// </summary>
    public string SingleLine
    {
        get
        {
            var parts = new List<string> { Street };

            if (!string.IsNullOrEmpty(BuildingNo))
                parts.Add($"No: {BuildingNo}");

            if (!string.IsNullOrEmpty(ApartmentNo))
                parts.Add($"Daire: {ApartmentNo}");

            parts.Add(District);
            parts.Add($"{PostalCode} {City}");
            parts.Add(Country);

            return string.Join(", ", parts);
        }
    }

    /// <summary>
    /// Kurye etiketi formatında
    /// </summary>
    public string CourierFormat => $"{Street} {(BuildingNo != null ? $"No:{BuildingNo}" : "")} {(ApartmentNo != null ? $"D:{ApartmentNo}" : "")}\n{District}\n{PostalCode} {City}\n{Country}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street.ToUpperInvariant();
        yield return District.ToUpperInvariant();
        yield return City.ToUpperInvariant();
        yield return PostalCode;
        yield return Country.ToUpperInvariant();
        yield return BuildingNo?.ToUpperInvariant();
        yield return ApartmentNo?.ToUpperInvariant();
    }

    public override string ToString() => SingleLine;
}