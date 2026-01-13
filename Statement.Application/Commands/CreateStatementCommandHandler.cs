using Statement.Application.DTOs;
using Statement.Domain.Entities;
using Statement.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Commands;

public class CreateStatementCommandHandler
    : IRequestHandler<CreateStatementCommand, Result<CardStatementDto>>
{
    private readonly ICardStatementRepository _repository;

    public CreateStatementCommandHandler(ICardStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CardStatementDto>> Handle(
        CreateStatementCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var statementResult = CardStatement.Create(
            dto.CardNumber,
            dto.CustomerName,
            dto.PeriodStartDate,
            dto.PeriodEndDate,
            dto.DueDate,
            dto.StatementDay,
            dto.PreviousBalance,
            dto.CreditLimit,
            dto.InterestRate,
            dto.CashAdvanceInterestRate,
            dto.CustomerEmail,
            dto.CustomerPhone);

        if (statementResult.IsFailure)
            return Result.Failure<CardStatementDto>(statementResult.Error!);

        var statement = statementResult.Value!;

        await _repository.AddAsync(statement, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(statement);
    }

    private static CardStatementDto MapToDto(CardStatement statement)
    {
        return new CardStatementDto
        {
            Id = statement.Id,
            StatementNumber = statement.StatementNumber,
            MaskedCardNumber = statement.MaskedCardNumber,
            CustomerName = statement.CustomerName,
            PeriodStartDate = statement.PeriodStartDate,
            PeriodEndDate = statement.PeriodEndDate,
            StatementDate = statement.StatementDate,
            DueDate = statement.DueDate,
            PreviousBalance = statement.PreviousBalance,
            TotalDebits = statement.TotalDebits,
            TotalCredits = statement.TotalCredits,
            CurrentBalance = statement.CurrentBalance,
            MinimumPayment = statement.MinimumPayment,
            AvailableCredit = statement.AvailableCredit,
            CreditLimit = statement.CreditLimit,
            InterestRate = statement.InterestRate,
            InterestAmount = statement.InterestAmount,
            Status = statement.Status.Name,
            StatusDisplayName = statement.Status.DisplayName,
            PaymentStatus = statement.PaymentStatus.Name,
            PaymentStatusDisplayName = statement.PaymentStatus.DisplayName,
            PaidAmount = statement.PaidAmount,
            RemainingBalance = statement.RemainingBalance,
            LastPaymentDate = statement.LastPaymentDate,
            PdfPath = statement.PdfPath,
            PdfGeneratedAt = statement.PdfGeneratedAt,
            CreatedAt = statement.CreatedAt,
            Items = new List<StatementItemDto>()
        };
    }
}