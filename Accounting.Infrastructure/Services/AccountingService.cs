using Accounting.Domain.Entities;
using Accounting.Domain.Enums;
using Accounting.Domain.Repositories;
using Accounting.Domain.Services;
using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Infrastructure.Services;

public class AccountingService : IAccountingService
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IAccountingPeriodRepository _periodRepository;
    private readonly IChartOfAccountRepository _accountRepository;
    private readonly ITrialBalanceService _trialBalanceService;

    // Standart hesap kodları (gerçek sistemde config'den alınır)
    private const string CardReceivablesAccount = "120.01"; // Kredi Kartı Alacakları
    private const string CardPayablesAccount = "320.01";    // Kredi Kartı Borçları
    private const string CashAccount = "100.01";            // Kasa/Banka
    private const string InterestIncomeAccount = "600.01";  // Faiz Geliri
    private const string CommissionIncomeAccount = "600.02"; // Komisyon Geliri
    private const string InterchangeIncomeAccount = "600.03"; // Interchange Geliri
    private const string MerchantPayablesAccount = "320.02"; // Üye İşyeri Borçları

    public AccountingService(
        IJournalEntryRepository journalRepository,
        IAccountingPeriodRepository periodRepository,
        IChartOfAccountRepository accountRepository,
        ITrialBalanceService trialBalanceService)
    {
        _journalRepository = journalRepository;
        _periodRepository = periodRepository;
        _accountRepository = accountRepository;
        _trialBalanceService = trialBalanceService;
    }

    public async Task<Result<JournalEntry>> PostCardPurchaseAsync(
        Guid transactionId,
        string cardNumber,
        decimal amount,
        string merchantName,
        CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetCurrentPeriodAsync(cancellationToken);
        if (period == null)
            return Result.Failure<JournalEntry>("Açık dönem bulunamadı");

        var receivablesAccount = await _accountRepository.GetByCodeAsync(CardReceivablesAccount, cancellationToken);
        var merchantPayablesAccount = await _accountRepository.GetByCodeAsync(MerchantPayablesAccount, cancellationToken);

        if (receivablesAccount == null || merchantPayablesAccount == null)
            return Result.Failure<JournalEntry>("Muhasebe hesapları bulunamadı");

        var description = $"Kart Alışverişi - {cardNumber[^4..]} - {merchantName}";

        var entryResult = JournalEntry.Create(
            DateTime.UtcNow,
            period.PeriodCode,
            TransactionType.CardPurchase,
            description,
            transactionId.ToString(),
            "Transaction",
            transactionId);

        if (entryResult.IsFailure)
            return Result.Failure<JournalEntry>(entryResult.Error!);

        var entry = entryResult.Value!;

        // Borç: Kredi Kartı Alacakları
        entry.AddLine(receivablesAccount.Id, receivablesAccount.AccountCode, receivablesAccount.AccountName,
            amount, 0, "Kart alacağı");

        // Alacak: Üye İşyeri Borçları
        entry.AddLine(merchantPayablesAccount.Id, merchantPayablesAccount.AccountCode, merchantPayablesAccount.AccountName,
            0, amount, "Üye işyeri borcu");

        // Fişi onayla
        entry.Post("System");

        // Bakiyeleri güncelle
        await UpdateBalancesForEntry(entry, period.PeriodCode, cancellationToken);

        await _journalRepository.AddAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return entry;
    }

    public async Task<Result<JournalEntry>> PostCardRefundAsync(
        Guid transactionId,
        string cardNumber,
        decimal amount,
        string merchantName,
        CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetCurrentPeriodAsync(cancellationToken);
        if (period == null)
            return Result.Failure<JournalEntry>("Açık dönem bulunamadı");

        var receivablesAccount = await _accountRepository.GetByCodeAsync(CardReceivablesAccount, cancellationToken);
        var merchantPayablesAccount = await _accountRepository.GetByCodeAsync(MerchantPayablesAccount, cancellationToken);

        if (receivablesAccount == null || merchantPayablesAccount == null)
            return Result.Failure<JournalEntry>("Muhasebe hesapları bulunamadı");

        var description = $"Kart İadesi - {cardNumber[^4..]} - {merchantName}";

        var entryResult = JournalEntry.Create(
            DateTime.UtcNow,
            period.PeriodCode,
            TransactionType.CardRefund,
            description,
            transactionId.ToString(),
            "Transaction",
            transactionId);

        if (entryResult.IsFailure)
            return Result.Failure<JournalEntry>(entryResult.Error!);

        var entry = entryResult.Value!;

        // Borç: Üye İşyeri Borçları (azaltma)
        entry.AddLine(merchantPayablesAccount.Id, merchantPayablesAccount.AccountCode, merchantPayablesAccount.AccountName,
            amount, 0, "Üye işyeri borcu düşümü");

        // Alacak: Kredi Kartı Alacakları (azaltma)
        entry.AddLine(receivablesAccount.Id, receivablesAccount.AccountCode, receivablesAccount.AccountName,
            0, amount, "Kart alacağı düşümü");

        entry.Post("System");
        await UpdateBalancesForEntry(entry, period.PeriodCode, cancellationToken);

        await _journalRepository.AddAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return entry;
    }

    public async Task<Result<JournalEntry>> PostCardPaymentAsync(
        Guid statementId,
        string cardNumber,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetCurrentPeriodAsync(cancellationToken);
        if (period == null)
            return Result.Failure<JournalEntry>("Açık dönem bulunamadı");

        var cashAccount = await _accountRepository.GetByCodeAsync(CashAccount, cancellationToken);
        var receivablesAccount = await _accountRepository.GetByCodeAsync(CardReceivablesAccount, cancellationToken);

        if (cashAccount == null || receivablesAccount == null)
            return Result.Failure<JournalEntry>("Muhasebe hesapları bulunamadı");

        var description = $"Kart Ödemesi - {cardNumber[^4..]}";

        var entryResult = JournalEntry.Create(
            DateTime.UtcNow,
            period.PeriodCode,
            TransactionType.CardPayment,
            description,
            statementId.ToString(),
            "Statement",
            statementId);

        if (entryResult.IsFailure)
            return Result.Failure<JournalEntry>(entryResult.Error!);

        var entry = entryResult.Value!;

        // Borç: Kasa/Banka
        entry.AddLine(cashAccount.Id, cashAccount.AccountCode, cashAccount.AccountName,
            amount, 0, "Tahsilat");

        // Alacak: Kredi Kartı Alacakları
        entry.AddLine(receivablesAccount.Id, receivablesAccount.AccountCode, receivablesAccount.AccountName,
            0, amount, "Alacak kapama");

        entry.Post("System");
        await UpdateBalancesForEntry(entry, period.PeriodCode, cancellationToken);

        await _journalRepository.AddAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return entry;
    }

    public async Task<Result<JournalEntry>> PostInterestAccrualAsync(
        Guid statementId,
        string cardNumber,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetCurrentPeriodAsync(cancellationToken);
        if (period == null)
            return Result.Failure<JournalEntry>("Açık dönem bulunamadı");

        var receivablesAccount = await _accountRepository.GetByCodeAsync(CardReceivablesAccount, cancellationToken);
        var interestIncomeAccount = await _accountRepository.GetByCodeAsync(InterestIncomeAccount, cancellationToken);

        if (receivablesAccount == null || interestIncomeAccount == null)
            return Result.Failure<JournalEntry>("Muhasebe hesapları bulunamadı");

        var description = $"Faiz Tahakkuku - {cardNumber[^4..]}";

        var entryResult = JournalEntry.Create(
            DateTime.UtcNow,
            period.PeriodCode,
            TransactionType.InterestAccrual,
            description,
            statementId.ToString(),
            "Statement",
            statementId);

        if (entryResult.IsFailure)
            return Result.Failure<JournalEntry>(entryResult.Error!);

        var entry = entryResult.Value!;

        // Borç: Kredi Kartı Alacakları
        entry.AddLine(receivablesAccount.Id, receivablesAccount.AccountCode, receivablesAccount.AccountName,
            amount, 0, "Faiz alacağı");

        // Alacak: Faiz Geliri
        entry.AddLine(interestIncomeAccount.Id, interestIncomeAccount.AccountCode, interestIncomeAccount.AccountName,
            0, amount, "Faiz geliri");

        entry.Post("System");
        await UpdateBalancesForEntry(entry, period.PeriodCode, cancellationToken);

        await _journalRepository.AddAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return entry;
    }

    public async Task<Result<JournalEntry>> PostCommissionIncomeAsync(
        Guid transactionId,
        string merchantId,
        decimal totalCommission,
        decimal bankShare,
        decimal interchangeFee,
        CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetCurrentPeriodAsync(cancellationToken);
        if (period == null)
            return Result.Failure<JournalEntry>("Açık dönem bulunamadı");

        var merchantPayablesAccount = await _accountRepository.GetByCodeAsync(MerchantPayablesAccount, cancellationToken);
        var commissionIncomeAccount = await _accountRepository.GetByCodeAsync(CommissionIncomeAccount, cancellationToken);
        var interchangeIncomeAccount = await _accountRepository.GetByCodeAsync(InterchangeIncomeAccount, cancellationToken);

        if (merchantPayablesAccount == null || commissionIncomeAccount == null || interchangeIncomeAccount == null)
            return Result.Failure<JournalEntry>("Muhasebe hesapları bulunamadı");

        var description = $"Komisyon Geliri - Üye İşyeri: {merchantId}";

        var entryResult = JournalEntry.Create(
            DateTime.UtcNow,
            period.PeriodCode,
            TransactionType.CommissionIncome,
            description,
            transactionId.ToString(),
            "Transaction",
            transactionId);

        if (entryResult.IsFailure)
            return Result.Failure<JournalEntry>(entryResult.Error!);

        var entry = entryResult.Value!;

        // Borç: Üye İşyeri Borçları (komisyon kesintisi)
        entry.AddLine(merchantPayablesAccount.Id, merchantPayablesAccount.AccountCode, merchantPayablesAccount.AccountName,
            totalCommission, 0, "Komisyon kesintisi");

        // Alacak: Komisyon Geliri (Banka payı)
        entry.AddLine(commissionIncomeAccount.Id, commissionIncomeAccount.AccountCode, commissionIncomeAccount.AccountName,
            0, bankShare, "Komisyon geliri");

        // Alacak: Interchange Geliri
        entry.AddLine(interchangeIncomeAccount.Id, interchangeIncomeAccount.AccountCode, interchangeIncomeAccount.AccountName,
            0, interchangeFee, "Interchange geliri");

        entry.Post("System");
        await UpdateBalancesForEntry(entry, period.PeriodCode, cancellationToken);

        await _journalRepository.AddAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return entry;
    }

    public async Task<Result<JournalEntry>> PostMerchantSettlementAsync(
        string merchantId,
        decimal grossAmount,
        decimal commission,
        decimal netAmount,
        CancellationToken cancellationToken = default)
    {
        var period = await _periodRepository.GetCurrentPeriodAsync(cancellationToken);
        if (period == null)
            return Result.Failure<JournalEntry>("Açık dönem bulunamadı");

        var merchantPayablesAccount = await _accountRepository.GetByCodeAsync(MerchantPayablesAccount, cancellationToken);
        var cashAccount = await _accountRepository.GetByCodeAsync(CashAccount, cancellationToken);

        if (merchantPayablesAccount == null || cashAccount == null)
            return Result.Failure<JournalEntry>("Muhasebe hesapları bulunamadı");

        var description = $"Üye İşyeri Hakediş - {merchantId}";

        var entryResult = JournalEntry.Create(
            DateTime.UtcNow,
            period.PeriodCode,
            TransactionType.MerchantSettlement,
            description,
            merchantId,
            "Merchant",
            null);

        if (entryResult.IsFailure)
            return Result.Failure<JournalEntry>(entryResult.Error!);

        var entry = entryResult.Value!;

        // Borç: Üye İşyeri Borçları (ödeme)
        entry.AddLine(merchantPayablesAccount.Id, merchantPayablesAccount.AccountCode, merchantPayablesAccount.AccountName,
            netAmount, 0, "Hakediş ödemesi");

        // Alacak: Kasa/Banka
        entry.AddLine(cashAccount.Id, cashAccount.AccountCode, cashAccount.AccountName,
            0, netAmount, "Hakediş transferi");

        entry.Post("System");
        await UpdateBalancesForEntry(entry, period.PeriodCode, cancellationToken);

        await _journalRepository.AddAsync(entry, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        return entry;
    }

    private async Task UpdateBalancesForEntry(JournalEntry entry, string periodCode, CancellationToken cancellationToken)
    {
        foreach (var line in entry.Lines)
        {
            await _trialBalanceService.UpdateAccountBalanceAsync(
                line.AccountId,
                periodCode,
                line.DebitAmount,
                line.CreditAmount,
                cancellationToken);
        }
    }
}