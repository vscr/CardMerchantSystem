using Dapper;
using Merchant.Domain.ReadModels;
using Merchant.Domain.Repositories;
using System.Data;

namespace Merchant.Infrastructure.Repositories;

public class MerchantDapperRepository :  IMerchantDapperRepository
{
    private readonly IDbConnection _connection;
    public MerchantDapperRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<MerchantReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                Id, MerchantCode, Name, TradeName, TaxNumber, TaxOffice, 
                MerchantTypeId, StatusId, PhoneNumber, Email, Address, City, District,
                IBAN, CommissionRate, ContractStartDate, ContractEndDate,
                ApprovedBy, ApprovedAt, RejectionReason,
                CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
            FROM Merchants
            WHERE Id = @Id";

        return await _connection.QuerySingleOrDefaultAsync<MerchantReadModel>(sql, new { Id = id });
    }

    public async Task<IEnumerable<MerchantReadModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                Id, MerchantCode, Name, TradeName, TaxNumber, TaxOffice, 
                MerchantTypeId, StatusId, PhoneNumber, Email, Address, City, District,
                IBAN, CommissionRate, ContractStartDate, ContractEndDate,
                ApprovedBy, ApprovedAt, RejectionReason,
                CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
            FROM Merchants
            ORDER BY Name";

        return await _connection.QueryAsync<MerchantReadModel>(sql);
    }

    public async Task<MerchantReadModel?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                Id, MerchantCode, Name, TradeName, TaxNumber, TaxOffice, 
                MerchantTypeId, StatusId, PhoneNumber, Email, Address, City, District,
                IBAN, CommissionRate, ContractStartDate, ContractEndDate,
                ApprovedBy, ApprovedAt, RejectionReason,
                CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
            FROM Merchants
            WHERE TaxNumber = @TaxNumber";

        return await _connection.QuerySingleOrDefaultAsync<MerchantReadModel>(sql, new { TaxNumber = taxNumber });
    }

    public async Task<int> GetActiveMerchantCountAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM Merchants
            WHERE StatusId = 3";

        var result = await _connection.ExecuteScalarAsync<int>(sql);
        return result;
    }

    public async Task<(IEnumerable<MerchantReadModel> Data, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Offset", (pageNumber - 1) * pageSize);
        parameters.Add("@PageSize", pageSize);

        var whereClause = "WHERE 1=1";

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            whereClause += @" AND (
                Name LIKE @SearchTerm OR 
                MerchantCode LIKE @SearchTerm OR 
                TaxNumber LIKE @SearchTerm
            )";
            parameters.Add("@SearchTerm", $"%{searchTerm}%");
        }

        var countSql = $@"
            SELECT COUNT(*)
            FROM Merchants
            {whereClause}";

        var dataSql = $@"
            SELECT 
                Id, MerchantCode, Name, TradeName, TaxNumber, TaxOffice, 
                MerchantTypeId, StatusId, PhoneNumber, Email, Address, City, District,
                IBAN, CommissionRate, ContractStartDate, ContractEndDate,
                ApprovedBy, ApprovedAt, RejectionReason,
                CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
            FROM Merchants
            {whereClause}
            ORDER BY Name
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        var totalCount = await _connection.ExecuteScalarAsync<int>(countSql, parameters);
        var data = await _connection.QueryAsync<MerchantReadModel>(dataSql, parameters);

        return (data, totalCount);
    }

    public async Task<MerchantDetailDto?> GetMerchantDetailAsync(Guid merchantId, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                m.Id,
                m.MerchantCode,
                m.Name,
                m.TaxNumber,
                m.StatusId,
                m.CreatedAt,
                COUNT(DISTINCT t.Id) as TotalTerminals,
                COUNT(DISTINCT CASE WHEN t.StatusId = 1 THEN t.Id END) as ActiveTerminals,
                ISNULL(SUM(trx.Amount), 0) as TotalTransactionAmount
            FROM Merchants m
            LEFT JOIN Terminals t ON t.MerchantId = m.Id
            LEFT JOIN Transactions trx ON trx.MerchantId = m.Id
            WHERE m.Id = @MerchantId
            GROUP BY m.Id, m.MerchantCode, m.Name, m.TaxNumber, m.StatusId, m.CreatedAt";

        return await _connection.QuerySingleOrDefaultAsync<MerchantDetailDto>(sql, new { MerchantId = merchantId });
    }
}