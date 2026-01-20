using Fee.Application.DTOs;
using Fee.Domain.Repositories;
using MediatR;

namespace Fee.Application.Queries;

public record GetMerchantTariffsQuery(string MerchantId) : IRequest<IReadOnlyList<MerchantTariffDto>>;
public class GetMerchantTariffsQueryHandler
    : IRequestHandler<GetMerchantTariffsQuery, IReadOnlyList<MerchantTariffDto>>
{
    private readonly IMerchantTariffRepository _merchantTariffRepository;
    private readonly ITariffRepository _tariffRepository;

    public GetMerchantTariffsQueryHandler(
        IMerchantTariffRepository merchantTariffRepository,
        ITariffRepository tariffRepository)
    {
        _merchantTariffRepository = merchantTariffRepository;
        _tariffRepository = tariffRepository;
    }

    public async Task<IReadOnlyList<MerchantTariffDto>> Handle(
        GetMerchantTariffsQuery request,
        CancellationToken cancellationToken)
    {
        var merchantTariffs = await _merchantTariffRepository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);
        var result = new List<MerchantTariffDto>();

        foreach (var mt in merchantTariffs)
        {
            var tariff = await _tariffRepository.GetByIdAsync(mt.TariffId, cancellationToken);

            result.Add(new MerchantTariffDto
            {
                Id = mt.Id,
                MerchantId = mt.MerchantId,
                TariffId = mt.TariffId,
                TariffCode = tariff?.TariffCode ?? "",
                TariffName = tariff?.TariffName ?? "",
                FeeType = mt.FeeType.Name,
                FeeTypeDisplayName = mt.FeeType.DisplayName,
                AssignedDate = mt.AssignedDate,
                EndDate = mt.EndDate,
                IsActive = mt.IsActive,
                SpecialRate = mt.SpecialRate,
                Notes = mt.Notes
            });
        }

        return result;
    }
}