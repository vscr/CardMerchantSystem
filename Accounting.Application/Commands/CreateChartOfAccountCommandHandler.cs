using Accounting.Application.DTOs;
using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Commands;

public class CreateChartOfAccountCommandHandler
    : IRequestHandler<CreateChartOfAccountCommand, Result<ChartOfAccountDto>>
{
    private readonly IChartOfAccountRepository _repository;

    public CreateChartOfAccountCommandHandler(IChartOfAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ChartOfAccountDto>> Handle(
        CreateChartOfAccountCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Aynı kodla hesap var mı?
        var existing = await _repository.GetByCodeAsync(dto.AccountCode, cancellationToken);
        if (existing != null)
            return Result.Failure<ChartOfAccountDto>("Bu hesap kodu zaten mevcut");

        var accountType = AccountType.FromId<AccountType>(dto.AccountTypeId);
        if (accountType == null)
            return Result.Failure<ChartOfAccountDto>("Geçersiz hesap tipi");

        var accountResult = ChartOfAccount.Create(
            dto.AccountCode,
            dto.AccountName,
            accountType,
            dto.Level,
            dto.ParentAccountId,
            dto.Description,
            dto.IsPostable,
            dto.CurrencyCode);

        if (accountResult.IsFailure)
            return Result.Failure<ChartOfAccountDto>(accountResult.Error!);

        var account = accountResult.Value!;

        await _repository.AddAsync(account, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(account);
    }

    private static ChartOfAccountDto MapToDto(ChartOfAccount account)
    {
        return new ChartOfAccountDto
        {
            Id = account.Id,
            AccountCode = account.AccountCode,
            AccountName = account.AccountName,
            Description = account.Description,
            AccountType = account.AccountType.Name,
            AccountTypeDisplayName = account.AccountType.DisplayName,
            ParentAccountId = account.ParentAccountId,
            Level = account.Level,
            IsActive = account.IsActive,
            IsPostable = account.IsPostable,
            CurrencyCode = account.CurrencyCode,
            CreatedAt = account.CreatedAt
        };
    }
}