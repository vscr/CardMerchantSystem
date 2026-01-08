using Merchant.Application.DTOs;
using Merchant.Domain.Entities;
using Merchant.Domain.Enums;
using Merchant.Domain.Repositories;
using MediatR;

namespace Merchant.Application.Queries;

public class GetMerchantsByStatusQueryHandler
    : IRequestHandler<GetMerchantsByStatusQuery, IReadOnlyList<MerchantDto>>
{
    private readonly IMerchantRepository _repository;

    public GetMerchantsByStatusQueryHandler(IMerchantRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MerchantDto>> Handle(
        GetMerchantsByStatusQuery request,
        CancellationToken cancellationToken)
    {
        var status = MerchantStatus.FromId<MerchantStatus>(request.StatusId);

        if (status == null)
            return new List<MerchantDto>();

        var merchants = await _repository.GetByStatusAsync(status, cancellationToken);

        return merchants.Select(MapToDto).ToList();
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
            Status = m.Status.Name,
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