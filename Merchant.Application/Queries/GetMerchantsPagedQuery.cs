using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Merchant.Application.DTOs;
using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Merchant.Application.Queries;

/// <summary>
/// Sayfalı üye işyeri listesi query'si
/// </summary>
public record GetMerchantsPagedQuery(MerchantFilterDto Filter) : IRequest<PagedResponse<MerchantDto>>;

public class GetMerchantsPagedQueryHandler : IRequestHandler<GetMerchantsPagedQuery, PagedResponse<MerchantDto>>
{
    private readonly IMerchantRepository _repository;
    private readonly ILogger<GetMerchantsPagedQueryHandler> _logger;

    public GetMerchantsPagedQueryHandler(
        IMerchantRepository repository,
        ILogger<GetMerchantsPagedQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<MerchantDto>> Handle(
        GetMerchantsPagedQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching merchants page {PageNumber} with size {PageSize}",
            request.Filter.PageNumber,
            request.Filter.PageSize);

        // Status enum'ı çözümle
        MerchantStatus? status = null;
        if (request.Filter.StatusId.HasValue)
        {
            status = MerchantStatus.FromId<MerchantStatus>(request.Filter.StatusId.Value);
        }

        var (items, totalCount) = await _repository.GetPagedAsync(
            request.Filter.PageNumber,
            request.Filter.PageSize,
            status,
            request.Filter.SearchTerm,
            request.Filter.SortBy,
            request.Filter.SortDescending,
            cancellationToken);

        var dtos = items.Select(MapToDto).ToList();

        _logger.LogInformation(
            "Retrieved {Count} merchants out of {Total}",
            dtos.Count,
            totalCount);

        return PagedResponse<MerchantDto>.Create(
            dtos,
            totalCount,
            request.Filter.PageNumber,
            request.Filter.PageSize);
    }

    private static MerchantDto MapToDto(MerchantAggregate m)
    {
        return new MerchantDto
        {
            Id = m.Id,
            MerchantCode = m.MerchantCode.Value,
            Name = m.Name,
            TradeName = m.TradeName,
            TaxNumber = m.TaxNumber.Masked,
            TaxOffice = m.TaxOffice,
            MerchantType = m.MerchantType.Name,
            MerchantTypeId = m.MerchantType.Id,
            Status = m.Status.Name,
            StatusId = m.Status.Id,
            StatusDisplayName = m.Status.DisplayName,
            PhoneNumber = m.PhoneNumber,
            Email = m.Email,
            Address = m.Address,
            City = m.City,
            District = m.District,
            IBAN = m.IBAN.Masked,
            CommissionRate = m.CommissionRate,
            ContractStartDate = m.ContractStartDate,
            ContractEndDate = m.ContractEndDate,
            ApprovedBy = m.ApprovedBy,
            ApprovedAt = m.ApprovedAt,
            RejectionReason = m.RejectionReason,
            ActiveTerminalCount = m.ActiveTerminalCount,
            CreatedAt = m.CreatedAt,
            UpdatedAt = m.UpdatedAt
        };
    }
}