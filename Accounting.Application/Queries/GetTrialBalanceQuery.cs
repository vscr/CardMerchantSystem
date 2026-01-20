using Accounting.Application.DTOs;
using Accounting.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Accounting.Application.Queries;

public record GetTrialBalanceQuery(string PeriodCode) : IRequest<Result<TrialBalanceDto>>; public class GetTrialBalanceQueryHandler
    : IRequestHandler<GetTrialBalanceQuery, Result<TrialBalanceDto>>
{
    private readonly IAccountBalanceRepository _balanceRepository;
    private readonly IAccountingPeriodRepository _periodRepository;
    private readonly IChartOfAccountRepository _accountRepository;

    public GetTrialBalanceQueryHandler(
        IAccountBalanceRepository balanceRepository,
        IAccountingPeriodRepository periodRepository,
        IChartOfAccountRepository accountRepository)
    {
        _balanceRepository = balanceRepository;
        _periodRepository = periodRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Result<TrialBalanceDto>> Handle(
        GetTrialBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var period = await _periodRepository.GetByCodeAsync(request.PeriodCode, cancellationToken);
        if (period == null)
            return Result.Failure<TrialBalanceDto>("Dönem bulunamadı", ErrorCodes.NotFound);

        var balances = await _balanceRepository.GetByPeriodAsync(request.PeriodCode, cancellationToken);
        var accounts = await _accountRepository.GetAllActiveAsync(cancellationToken);

        var accountDict = accounts.ToDictionary(a => a.Id, a => a.AccountName);

        var trialBalance = new TrialBalanceDto
        {
            PeriodCode = period.PeriodCode,
            PeriodName = period.PeriodName,
            GeneratedAt = DateTime.UtcNow,
            TotalOpeningDebit = balances.Sum(b => b.OpeningDebit),
            TotalOpeningCredit = balances.Sum(b => b.OpeningCredit),
            TotalPeriodDebit = balances.Sum(b => b.PeriodDebit),
            TotalPeriodCredit = balances.Sum(b => b.PeriodCredit),
            TotalClosingDebit = balances.Sum(b => b.ClosingDebit),
            TotalClosingCredit = balances.Sum(b => b.ClosingCredit),
            Accounts = balances.Select(b => new AccountBalanceDto
            {
                Id = b.Id,
                AccountId = b.AccountId,
                AccountCode = b.AccountCode,
                AccountName = accountDict.GetValueOrDefault(b.AccountId, b.AccountCode),
                PeriodCode = b.PeriodCode,
                OpeningDebit = b.OpeningDebit,
                OpeningCredit = b.OpeningCredit,
                PeriodDebit = b.PeriodDebit,
                PeriodCredit = b.PeriodCredit,
                ClosingDebit = b.ClosingDebit,
                ClosingCredit = b.ClosingCredit,
                NetBalance = b.NetBalance,
                LastUpdated = b.LastUpdated
            }).OrderBy(a => a.AccountCode).ToList()
        };

        return trialBalance;
    }
}