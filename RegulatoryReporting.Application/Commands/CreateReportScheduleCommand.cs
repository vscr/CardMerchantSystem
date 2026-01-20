using CardMerchantSystem.Shared.Kernel;
using MediatR;
using RegulatoryReporting.Application.DTOs;
using RegulatoryReporting.Domain.Entities;
using RegulatoryReporting.Domain.Repositories;

namespace RegulatoryReporting.Application.Commands;

public record CreateReportScheduleCommand(CreateReportScheduleDto Dto) : IRequest<Result<ReportScheduleDto>>;

public class CreateReportScheduleCommandHandler : IRequestHandler<CreateReportScheduleCommand, Result<ReportScheduleDto>>
{
    private readonly IReportScheduleRepository _scheduleRepository;
    private readonly IReportDefinitionRepository _definitionRepository;

    public CreateReportScheduleCommandHandler(
        IReportScheduleRepository scheduleRepository,
        IReportDefinitionRepository definitionRepository)
    {
        _scheduleRepository = scheduleRepository;
        _definitionRepository = definitionRepository;
    }

    public async Task<Result<ReportScheduleDto>> Handle(CreateReportScheduleCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var definition = await _definitionRepository.GetByIdAsync(dto.ReportDefinitionId, cancellationToken);
        if (definition is null)
            return Result.Failure<ReportScheduleDto>("Rapor tanımı bulunamadı");

        var existing = await _scheduleRepository.GetByDefinitionIdAsync(dto.ReportDefinitionId, cancellationToken);
        if (existing is not null)
            return Result.Failure<ReportScheduleDto>("Bu rapor için zamanlama zaten mevcut");

        var scheduleResult = ReportSchedule.Create(
            dto.ReportDefinitionId,
            dto.DayOfMonth,
            dto.ExecutionTime,
            dto.PeriodStart,
            dto.PeriodEnd);

        if (scheduleResult.IsFailure)
            return Result.Failure<ReportScheduleDto>(scheduleResult.Error);

        var schedule = scheduleResult.Value!;

        if (dto.DayOfWeek.HasValue)
            schedule.SetWeeklySchedule(dto.DayOfWeek.Value);

        await _scheduleRepository.AddAsync(schedule, cancellationToken);
        await _scheduleRepository.SaveChangesAsync(cancellationToken);

        return new ReportScheduleDto
        {
            Id = schedule.Id,
            ReportDefinitionId = schedule.ReportDefinitionId,
            ReportDefinitionName = definition.Name,
            DayOfMonth = schedule.DayOfMonth,
            DayOfWeek = schedule.DayOfWeek,
            ExecutionTime = schedule.ExecutionTime,
            PeriodStart = schedule.PeriodStart,
            PeriodEnd = schedule.PeriodEnd,
            IsEnabled = schedule.IsEnabled,
            LastRunAt = schedule.LastRunAt,
            NextRunAt = schedule.NextRunAt,
            ConsecutiveFailures = schedule.ConsecutiveFailures,
            LastErrorMessage = schedule.LastErrorMessage,
            CreatedAt = schedule.CreatedAt
        };
    }
}