using Accounting.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Accounting.Domain.Entities;

/// <summary>
/// Hesap Planı
/// </summary>
public class ChartOfAccount : AggregateRoot
{
    public string AccountCode { get; private set; } = null!;
    public string AccountName { get; private set; } = null!;
    public string? Description { get; private set; }
    public AccountType AccountType { get; private set; } = null!;
    public Guid? ParentAccountId { get; private set; }
    public int Level { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsPostable { get; private set; } // Alt hesaba kayıt yapılabilir mi?
    public string? CurrencyCode { get; private set; }

    // EF Core için
    private ChartOfAccount() { }

    /// <summary>
    /// Yeni hesap oluşturur
    /// </summary>
    public static Result<ChartOfAccount> Create(
        string accountCode,
        string accountName,
        AccountType accountType,
        int level,
        Guid? parentAccountId = null,
        string? description = null,
        bool isPostable = true,
        string? currencyCode = "TRY")
    {
        if (string.IsNullOrWhiteSpace(accountCode))
            return Result.Failure<ChartOfAccount>("Hesap kodu boş olamaz");

        if (string.IsNullOrWhiteSpace(accountName))
            return Result.Failure<ChartOfAccount>("Hesap adı boş olamaz");

        if (level < 1 || level > 5)
            return Result.Failure<ChartOfAccount>("Hesap seviyesi 1-5 arasında olmalı");

        var account = new ChartOfAccount
        {
            AccountCode = accountCode,
            AccountName = accountName,
            AccountType = accountType,
            ParentAccountId = parentAccountId,
            Level = level,
            Description = description,
            IsActive = true,
            IsPostable = isPostable,
            CurrencyCode = currencyCode
        };

        return account;
    }

    public void Update(string accountName, string? description)
    {
        AccountName = accountName;
        Description = description;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void SetPostable(bool isPostable) => IsPostable = isPostable;
}