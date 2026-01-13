using Statement.Application.DTOs;
using Statement.Domain.Enums;
using Statement.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Commands;

public class AddStatementItemCommandHandler
    : IRequestHandler<AddStatementItemCommand, Result<CardStatementDto>>
{
    private readonly ICardStatementRepository _repository;

    public AddStatementItemCommandHandler(ICardStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CardStatementDto>> Handle(
        AddStatementItemCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var statement = await _repository.GetByIdWithItemsAsync(dto.StatementId, cancellationToken);
        if (statement == null)
            return Result.Failure<CardStatementDto>("Ekstre bulunamadı");

        var itemType = StatementItemType.FromId<StatementItemType>(dto.ItemTypeId);
        if (itemType == null)
            return Result.Failure<CardStatementDto>("Geçersiz kalem tipi");

        var itemResult = statement.AddItem(
            itemType,
            dto.TransactionDate,
            dto.Description,
            dto.Amount,
            dto.ReferenceNumber,
            dto.MerchantName,
            dto.InstallmentNumber,
            dto.TotalInstallments);

        if (itemResult.IsFailure)
            return Result.Failure<CardStatementDto>(itemResult.Error!);

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