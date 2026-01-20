using HSM.Application.DTOs;
using HSM.Domain.Repositories;
using MediatR;

namespace HSM.Application.Queries;

public record GetCommandLogsQuery(DateTime StartDate, DateTime EndDate) : IRequest<IReadOnlyList<HSMCommandLogDto>>;
public class GetCommandLogsQueryHandler
    : IRequestHandler<GetCommandLogsQuery, IReadOnlyList<HSMCommandLogDto>>
{
    private readonly IHSMCommandLogRepository _logRepository;
    private readonly IHSMDeviceRepository _deviceRepository;

    public GetCommandLogsQueryHandler(
        IHSMCommandLogRepository logRepository,
        IHSMDeviceRepository deviceRepository)
    {
        _logRepository = logRepository;
        _deviceRepository = deviceRepository;
    }

    public async Task<IReadOnlyList<HSMCommandLogDto>> Handle(
        GetCommandLogsQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _logRepository.GetByDateRangeAsync(request.StartDate, request.EndDate, cancellationToken);
        var result = new List<HSMCommandLogDto>();

        foreach (var log in logs)
        {
            var device = await _deviceRepository.GetByIdAsync(log.HSMDeviceId, cancellationToken);

            result.Add(new HSMCommandLogDto
            {
                Id = log.Id,
                HSMDeviceId = log.HSMDeviceId,
                HSMDeviceName = device?.DeviceName,
                CommandType = log.CommandType.Name,
                CommandTypeDisplayName = log.CommandType.DisplayName,
                ResponseCode = log.ResponseCode,
                IsSuccess = log.IsSuccess,
                ErrorMessage = log.ErrorMessage,
                ExecutionTimeMs = log.ExecutionTimeMs,
                ExecutedAt = log.ExecutedAt,
                ReferenceId = log.ReferenceId,
                CardNumberMasked = log.CardNumberMasked
            });
        }

        return result;
    }
}