using Accounting.Domain.Entities;
using Accounting.Domain.Repositories;
using Accounting.Domain.Services;
using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Infrastructure.Services;

public class TrialBalanceService : ITrialBalanceService
{
    private readonly IAccountBalanceRepository _balanceRepository;
    private readonly IChartOfAccountRepository _accountRepository;

    public TrialBalanceService(
        IAccountBalanceRepository balanceRepository,
        IChartOfAccountRepository accountRepository)
    {
        _balanceRepository = balanceRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Result<IReadOnlyList<AccountBalance>>> GenerateTrialBalanceAsync(
        string periodCode,
        CancellationToken cancellationToken = default)
    {
        var balances = await _balanceRepository.GetByPeriodAsync(periodCode, cancellationToken);
        return Result.Success(balances);
    }

    public async Task<Result> UpdateAccountBalanceAsync(
        Guid accountId,
        string periodCode,
        decimal debitAmount,
        decimal creditAmount,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account == null)
            return Result.Failure("Hesap bulunamadı");

        var balance = await _balanceRepository.GetByAccountAndPeriodAsync(accountId, periodCode, cancellationToken);

        if (balance == null)
        {
            // Yeni bakiye kaydı oluştur
            balance = AccountBalance.Create(accountId, account.AccountCode, periodCode);
            await _balanceRepository.AddAsync(balance, cancellationToken);
        }

        // Bakiyeyi güncelle
        if (debitAmount > 0)
            balance.AddDebit(debitAmount);

        if (creditAmount > 0)
            balance.AddCredit(creditAmount);

        await _balanceRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> CarryForwardBalancesAsync(
        string fromPeriodCode,
        string toPeriodCode,
        CancellationToken cancellationToken = default)
    {
        var balances = await _balanceRepository.GetNonZeroBalancesByPeriodAsync(fromPeriodCode, cancellationToken);

        foreach (var balance in balances)
        {
            // Yeni dönem için bakiye oluştur
            var newBalance = AccountBalance.Create(
                balance.AccountId,
                balance.AccountCode,
                toPeriodCode,
                balance.ClosingDebit,
                balance.ClosingCredit);

            await _balanceRepository.AddAsync(newBalance, cancellationToken);
        }

        await _balanceRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}