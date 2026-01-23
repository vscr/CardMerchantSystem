// Merchant.Domain/Repositories/IMerchantDapperRepository.cs - GÜNCELLE

using Merchant.Domain.ReadModels;

namespace Merchant.Domain.Repositories;

public interface IMerchantDapperRepository
{
    Task<MerchantReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MerchantReadModel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<MerchantReadModel?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default);
    Task<int> GetActiveMerchantCountAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<MerchantReadModel> Data, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);
    Task<MerchantDetailDto?> GetMerchantDetailAsync(Guid merchantId, CancellationToken cancellationToken = default);
}

public class MerchantDetailDto
{
    public Guid Id { get; set; }
    public string MerchantCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TaxNumber { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalTerminals { get; set; }
    public int ActiveTerminals { get; set; }
    public decimal TotalTransactionAmount { get; set; }
}