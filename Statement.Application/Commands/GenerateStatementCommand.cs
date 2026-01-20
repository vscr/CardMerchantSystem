using CardMerchantSystem.Shared.Kernel;
using MediatR;
using Statement.Application.DTOs;
using Statement.Domain.Repositories;

namespace Statement.Application.Commands;

public record GenerateStatementCommand(Guid StatementId) : IRequest<Result<CardStatementDto>>;
public class GenerateStatementCommandHandler
    : IRequestHandler<GenerateStatementCommand, Result<CardStatementDto>>
{
    private readonly ICardStatementRepository _repository;

    public GenerateStatementCommandHandler(ICardStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CardStatementDto>> Handle(
        GenerateStatementCommand request,
        CancellationToken cancellationToken)
    {
        var statement = await _repository.GetByIdWithItemsAsync(request.StatementId, cancellationToken);
        if (statement == null)
            return Result.Failure<CardStatementDto>("Ekstre bulunamadı");

        // Faiz hesapla
        var interestResult = statement.CalculateInterest();
        if (interestResult.IsFailure)
            return Result.Failure<CardStatementDto>(interestResult.Error!);

        // Ekstreyi oluştur
        var generateResult = statement.Generate();
        if (generateResult.IsFailure)
            return Result.Failure<CardStatementDto>(generateResult.Error!);

        await _repository.UpdateAsync(statement, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(statement);
    }

    private static CardStatementDto MapToDto(Domain.Entities.CardStatement statement)
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
            Items = statement.Items.Select(i => new StatementItemDto
            {
                Id = i.Id,
                ItemType = i.ItemType.Name,
                ItemTypeDisplayName = i.ItemType.DisplayName,
                TransactionDate = i.TransactionDate,
                PostDate = i.PostDate,
                Description = i.Description,
                Amount = i.Amount,
                ReferenceNumber = i.ReferenceNumber,
                MerchantName = i.MerchantName,
                InstallmentInfo = i.InstallmentInfo,
                OriginalCurrency = i.OriginalCurrency,
                OriginalAmount = i.OriginalAmount,
                ExchangeRate = i.ExchangeRate
            }).ToList()
        };
    }
}