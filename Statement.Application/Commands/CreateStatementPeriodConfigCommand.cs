using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Statement.Application.DTOs;
using Statement.Domain.Entities;
using Statement.Domain.Repositories;

namespace Statement.Application.Commands;

public record CreateStatementPeriodConfigCommand(CreateStatementPeriodConfigDto Dto) : IRequest<Result<StatementPeriodConfigDto>>;
public class CreateStatementPeriodConfigCommandHandler
    : IRequestHandler<CreateStatementPeriodConfigCommand, Result<StatementPeriodConfigDto>>
{
    private readonly IStatementPeriodConfigRepository _repository;

    public CreateStatementPeriodConfigCommandHandler(IStatementPeriodConfigRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<StatementPeriodConfigDto>> Handle(
        CreateStatementPeriodConfigCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Aynı kart için ayar var mı?
        var existing = await _repository.GetByCardNumberAsync(dto.CardNumber, cancellationToken);
        if (existing != null)
            return Result.Failure<StatementPeriodConfigDto>("Bu kart için kesim ayarı zaten mevcut");

        var configResult = StatementPeriodConfig.Create(
            dto.CardNumber,
            dto.StatementDay,
            dto.PaymentDueDays,
            dto.InterestRate,
            dto.CashAdvanceInterestRate,
            dto.MinimumPaymentRate,
            dto.MinimumPaymentAmount);

        if (configResult.IsFailure)
            return Result.Failure<StatementPeriodConfigDto>(configResult.Error!);

        var config = configResult.Value!;

        await _repository.AddAsync(config, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new StatementPeriodConfigDto
        {
            Id = config.Id,
            CardNumber = config.CardNumber,
            StatementDay = config.StatementDay,
            PaymentDueDays = config.PaymentDueDays,
            InterestRate = config.InterestRate,
            CashAdvanceInterestRate = config.CashAdvanceInterestRate,
            MinimumPaymentRate = config.MinimumPaymentRate,
            MinimumPaymentAmount = config.MinimumPaymentAmount,
            IsActive = config.IsActive,
            CreatedAt = config.CreatedAt
        };
    }
}