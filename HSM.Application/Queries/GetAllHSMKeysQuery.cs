using HSM.Application.DTOs;
using HSM.Domain.Repositories;
using MediatR;

namespace HSM.Application.Queries;

public record GetAllHSMKeysQuery() : IRequest<IReadOnlyList<HSMKeyDto>>;
public class GetAllHSMKeysQueryHandler
    : IRequestHandler<GetAllHSMKeysQuery, IReadOnlyList<HSMKeyDto>>
{
    private readonly IHSMKeyRepository _keyRepository;
    private readonly IHSMDeviceRepository _deviceRepository;

    public GetAllHSMKeysQueryHandler(
        IHSMKeyRepository keyRepository,
        IHSMDeviceRepository deviceRepository)
    {
        _keyRepository = keyRepository;
        _deviceRepository = deviceRepository;
    }

    public async Task<IReadOnlyList<HSMKeyDto>> Handle(
        GetAllHSMKeysQuery request,
        CancellationToken cancellationToken)
    {
        var keys = await _keyRepository.GetAllActiveAsync(cancellationToken);
        var result = new List<HSMKeyDto>();

        foreach (var key in keys)
        {
            var device = await _deviceRepository.GetByIdAsync(key.HSMDeviceId, cancellationToken);

            result.Add(new HSMKeyDto
            {
                Id = key.Id,
                KeyName = key.KeyName,
                KeyType = key.KeyType.Name,
                KeyTypeDisplayName = key.KeyType.DisplayName,
                KeyIndex = key.KeyIndex,
                KeyCheckValue = key.KeyCheckValue ?? "",
                KeyLength = key.KeyLength,
                IsActive = key.IsActive,
                ExpiryDate = key.ExpiryDate,
                Description = key.Description,
                HSMDeviceId = key.HSMDeviceId,
                HSMDeviceName = device?.DeviceName,
                IsExpired = key.IsExpired,
                CreatedAt = key.CreatedAt
            });
        }

        return result;
    }
}