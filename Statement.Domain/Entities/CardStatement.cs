using Statement.Domain.Enums;
using CardMerchantSystem.Shared.Kernel;

namespace Statement.Domain.Entities;

/// <summary>
/// Kart Ekstresi
/// </summary>
public class CardStatement : AggregateRoot
{
    public string StatementNumber { get; private set; } = null!;
    public string CardNumber { get; private set; } = null!;
    public string MaskedCardNumber { get; private set; } = null!;
    public string CustomerName { get; private set; } = null!;
    public string? CustomerEmail { get; private set; }
    public string? CustomerPhone { get; private set; }

    // Dönem Bilgileri
    public DateTime PeriodStartDate { get; private set; }
    public DateTime PeriodEndDate { get; private set; }
    public DateTime StatementDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public int StatementDay { get; private set; } // Her ayın kaçında kesim

    // Tutarlar
    public decimal PreviousBalance { get; private set; }
    public decimal TotalDebits { get; private set; }
    public decimal TotalCredits { get; private set; }
    public decimal CurrentBalance { get; private set; }
    public decimal MinimumPayment { get; private set; }
    public decimal AvailableCredit { get; private set; }
    public decimal CreditLimit { get; private set; }

    // Faiz Bilgileri
    public decimal InterestRate { get; private set; }
    public decimal InterestAmount { get; private set; }
    public decimal CashAdvanceInterestRate { get; private set; }

    // Durum
    public StatementStatus Status { get; private set; } = null!;
    public PaymentStatus PaymentStatus { get; private set; } = null!;
    public decimal PaidAmount { get; private set; }
    public DateTime? LastPaymentDate { get; private set; }

    // PDF
    public string? PdfPath { get; private set; }
    public DateTime? PdfGeneratedAt { get; private set; }

    // İlişkili kalemler
    private readonly List<StatementItem> _items = new();
    public IReadOnlyCollection<StatementItem> Items => _items.AsReadOnly();

    // EF Core için
    private CardStatement() { }

    /// <summary>
    /// Yeni ekstre oluşturur
    /// </summary>
    public static Result<CardStatement> Create(
        string cardNumber,
        string customerName,
        DateTime periodStartDate,
        DateTime periodEndDate,
        DateTime dueDate,
        int statementDay,
        decimal previousBalance,
        decimal creditLimit,
        decimal interestRate,
        decimal cashAdvanceInterestRate = 0,
        string? customerEmail = null,
        string? customerPhone = null)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return Result.Failure<CardStatement>("Kart numarası boş olamaz");

        if (periodEndDate <= periodStartDate)
            return Result.Failure<CardStatement>("Dönem bitiş tarihi başlangıçtan büyük olmalı");

        if (dueDate <= periodEndDate)
            return Result.Failure<CardStatement>("Son ödeme tarihi dönem bitişinden büyük olmalı");

        var statement = new CardStatement
        {
            StatementNumber = GenerateStatementNumber(),
            CardNumber = cardNumber,
            MaskedCardNumber = MaskCardNumber(cardNumber),
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            PeriodStartDate = periodStartDate,
            PeriodEndDate = periodEndDate,
            StatementDate = periodEndDate,
            DueDate = dueDate,
            StatementDay = statementDay,
            PreviousBalance = previousBalance,
            TotalDebits = 0,
            TotalCredits = 0,
            CurrentBalance = previousBalance,
            MinimumPayment = 0,
            CreditLimit = creditLimit,
            AvailableCredit = creditLimit - previousBalance,
            InterestRate = interestRate,
            InterestAmount = 0,
            CashAdvanceInterestRate = cashAdvanceInterestRate,
            Status = StatementStatus.Draft,
            PaymentStatus = PaymentStatus.Current,
            PaidAmount = 0
        };

        return statement;
    }

    /// <summary>
    /// Ekstre kalemi ekler
    /// </summary>
    public Result<StatementItem> AddItem(
        StatementItemType itemType,
        DateTime transactionDate,
        string description,
        decimal amount,
        string? referenceNumber = null,
        string? merchantName = null,
        int? installmentNumber = null,
        int? totalInstallments = null)
    {
        if (Status != StatementStatus.Draft)
            return Result.Failure<StatementItem>("Sadece taslak ekstrelere kalem eklenebilir");

        var item = StatementItem.Create(
            Id,
            itemType,
            transactionDate,
            description,
            amount,
            referenceNumber,
            merchantName,
            installmentNumber,
            totalInstallments);

        if (item.IsFailure)
            return Result.Failure<StatementItem>(item.Error!);

        _items.Add(item.Value!);

        // Toplamları güncelle
        RecalculateTotals();

        return item.Value!;
    }

    /// <summary>
    /// Toplamları yeniden hesaplar
    /// </summary>
    private void RecalculateTotals()
    {
        TotalDebits = _items.Where(x => x.ItemType.IsDebit).Sum(x => x.Amount);
        TotalCredits = _items.Where(x => x.ItemType.IsCredit).Sum(x => Math.Abs(x.Amount));

        CurrentBalance = PreviousBalance + TotalDebits - TotalCredits;
        AvailableCredit = CreditLimit - CurrentBalance;

        // Minimum ödeme: Bakiyenin %20'si veya 100 TL (hangisi büyükse)
        if (CurrentBalance > 0)
        {
            MinimumPayment = Math.Max(CurrentBalance * 0.20m, Math.Min(100, CurrentBalance));
            MinimumPayment = Math.Round(MinimumPayment, 2);
        }
        else
        {
            MinimumPayment = 0;
        }
    }

    /// <summary>
    /// Faiz hesaplar ve ekler
    /// </summary>
    public Result CalculateInterest()
    {
        if (Status != StatementStatus.Draft)
            return Result.Failure("Sadece taslak ekstrelerde faiz hesaplanabilir");

        if (PreviousBalance <= 0)
            return Result.Success();

        // Aylık faiz oranı
        var monthlyRate = InterestRate / 12 / 100;
        InterestAmount = Math.Round(PreviousBalance * monthlyRate, 2);

        if (InterestAmount > 0)
        {
            var interestItem = StatementItem.Create(
                Id,
                StatementItemType.Interest,
                StatementDate,
                $"Dönem Faizi (%{InterestRate:F2} yıllık)",
                InterestAmount);

            if (interestItem.IsSuccess)
            {
                _items.Add(interestItem.Value!);
                RecalculateTotals();
            }
        }

        return Result.Success();
    }

    /// <summary>
    /// Ekstreyi oluşturuldu olarak işaretle
    /// </summary>
    public Result Generate()
    {
        if (Status != StatementStatus.Draft)
            return Result.Failure("Sadece taslak ekstreler oluşturulabilir");

        if (!_items.Any())
            return Result.Failure("En az bir kalem eklenmeli");

        Status = StatementStatus.Generated;
        return Result.Success();
    }

    /// <summary>
    /// PDF yolunu ayarlar
    /// </summary>
    public void SetPdfPath(string pdfPath)
    {
        PdfPath = pdfPath;
        PdfGeneratedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ekstreyi gönderildi olarak işaretle
    /// </summary>
    public Result MarkAsSent()
    {
        if (Status != StatementStatus.Generated)
            return Result.Failure("Sadece oluşturulmuş ekstreler gönderilebilir");

        Status = StatementStatus.Sent;
        return Result.Success();
    }

    /// <summary>
    /// Ödeme kaydı
    /// </summary>
    public Result RecordPayment(decimal amount)
    {
        if (!Status.RequiresPayment)
            return Result.Failure("Bu ekstre için ödeme alınamaz");

        if (amount <= 0)
            return Result.Failure("Ödeme tutarı sıfırdan büyük olmalı");

        PaidAmount += amount;
        LastPaymentDate = DateTime.UtcNow;

        var remaining = CurrentBalance - PaidAmount;

        if (remaining <= 0)
        {
            Status = StatementStatus.Paid;
            PaymentStatus = PaymentStatus.Current;
        }
        else
        {
            Status = StatementStatus.PartiallyPaid;
        }

        return Result.Success();
    }

    /// <summary>
    /// Gecikme durumunu günceller
    /// </summary>
    public void UpdatePaymentStatus()
    {
        if (Status == StatementStatus.Paid || Status == StatementStatus.Cancelled)
            return;

        var today = DateTime.UtcNow.Date;
        var daysOverdue = (int)(today - DueDate).TotalDays;

        PaymentStatus = PaymentStatus.FromDaysOverdue(daysOverdue);

        if (PaymentStatus.IsOverdue && Status != StatementStatus.Overdue)
        {
            Status = StatementStatus.Overdue;
        }
    }

    /// <summary>
    /// Kalan borç
    /// </summary>
    public decimal RemainingBalance => CurrentBalance - PaidAmount;

    private static string GenerateStatementNumber()
    {
        return $"STM{DateTime.UtcNow:yyyyMMdd}{new Random().Next(100000, 999999)}";
    }

    private static string MaskCardNumber(string cardNumber)
    {
        if (cardNumber.Length < 10) return cardNumber;
        return $"{cardNumber[..6]}******{cardNumber[^4..]}";
    }
}