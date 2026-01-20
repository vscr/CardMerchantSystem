using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Merchant.Application.DTOs;
using Merchant.Domain.Entities;
using Merchant.Domain.Repositories;

namespace Merchant.Application.Queries;

public record GetMerchantWithTerminalsQuery(Guid Id) : IRequest<Result<MerchantWithTerminalsDto>>;
public class GetMerchantWithTerminalsQueryHandler
    : IRequestHandler<GetMerchantWithTerminalsQuery, Result<MerchantWithTerminalsDto>>
{
    private readonly IMerchantRepository _repository;

    public GetMerchantWithTerminalsQueryHandler(IMerchantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantWithTerminalsDto>> Handle(
        GetMerchantWithTerminalsQuery request,
        CancellationToken cancellationToken)
    {
        var merchant = await _repository.GetByIdWithTerminalsAsync(request.Id, cancellationToken);

        if (merchant == null)
            return Result.Failure<MerchantWithTerminalsDto>("Üye işyeri bulunamadı", ErrorCodes.MerchantNotFound);

        var dto = new MerchantWithTerminalsDto
        {
            Merchant = MapMerchantToDto(merchant),
            Terminals = merchant.Terminals.Select(MapTerminalToDto).ToList()
        };

        return dto;
    }

    private static MerchantDto MapMerchantToDto(MerchantAggregate m)
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

    private static TerminalDto MapTerminalToDto(Terminal t)
    {
        return new TerminalDto
        {
            Id = t.Id,
            MerchantId = t.MerchantId,
            TerminalCode = t.TerminalCode.Value,
            TerminalType = t.TerminalType.Name,
            Status = t.Status.Name,
            StatusDisplayName = t.Status.DisplayName,
            SerialNumber = t.SerialNumber,
            Model = t.Model,
            Location = t.Location,
            InstalledAt = t.InstalledAt,
            InstalledBy = t.InstalledBy,
            CreatedAt = t.CreatedAt
        };
    }
}

public class MerchantWithTerminalsDto
{
    public MerchantDto Merchant { get; set; } = null!;
    public List<TerminalDto> Terminals { get; set; } = new();
}