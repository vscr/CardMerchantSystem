namespace Transaction.Infrastructure.Dapper;

/// <summary>
/// Transaction modülü için optimize edilmiş SQL sorguları
/// </summary>
public static class TransactionQueries
{
    /// <summary>
    /// Sayfalı işlem listesi (WITH NOLOCK)
    /// </summary>
    public const string GetPagedTransactions = @"
        -- Count query (ayrı ve hızlı)
        SELECT COUNT(*) 
        FROM Transactions WITH (NOLOCK)
        WHERE (@MerchantId IS NULL OR MerchantId = @MerchantId)
          AND (@TerminalId IS NULL OR TerminalId = @TerminalId)
          AND (@CardNumberMasked IS NULL OR CardNumberMasked = @CardNumberMasked)
          AND (@StatusId IS NULL OR StatusId = @StatusId)
          AND (@TransactionTypeId IS NULL OR TransactionTypeId = @TransactionTypeId)
          AND (@StartDate IS NULL OR CreatedAt >= @StartDate)
          AND (@EndDate IS NULL OR CreatedAt <= @EndDate)
          AND (@MinAmount IS NULL OR Amount >= @MinAmount)
          AND (@MaxAmount IS NULL OR Amount <= @MaxAmount);

        -- Data query with pagination
        SELECT 
            Id,
            ReferenceNumber,
            TransactionTypeId,
            StatusId,
            Amount,
            Currency,
            AuthorizationCode,
            CardNumberMasked,
            MerchantId,
            MerchantCode,
            TerminalId,
            TerminalCode,
            DeclineReasonId,
            ErrorMessage,
            FraudCheckResultId,
            FraudScore,
            OriginalTransactionId,
            SettledAt,
            BatchNumber,
            CreatedAt
        FROM Transactions WITH (NOLOCK)
        WHERE (@MerchantId IS NULL OR MerchantId = @MerchantId)
          AND (@TerminalId IS NULL OR TerminalId = @TerminalId)
          AND (@CardNumberMasked IS NULL OR CardNumberMasked = @CardNumberMasked)
          AND (@StatusId IS NULL OR StatusId = @StatusId)
          AND (@TransactionTypeId IS NULL OR TransactionTypeId = @TransactionTypeId)
          AND (@StartDate IS NULL OR CreatedAt >= @StartDate)
          AND (@EndDate IS NULL OR CreatedAt <= @EndDate)
          AND (@MinAmount IS NULL OR Amount >= @MinAmount)
          AND (@MaxAmount IS NULL OR Amount <= @MaxAmount)
        ORDER BY CreatedAt DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    ";

    /// <summary>
    /// ID ile işlem getir
    /// </summary>
    public const string GetById = @"
        SELECT 
            Id,
            ReferenceNumber,
            TransactionTypeId,
            StatusId,
            Amount,
            Currency,
            AuthorizationCode,
            CardNumberMasked,
            MerchantId,
            MerchantCode,
            TerminalId,
            TerminalCode,
            DeclineReasonId,
            ErrorMessage,
            FraudCheckResultId,
            FraudScore,
            OriginalTransactionId,
            SettledAt,
            BatchNumber,
            CreatedAt
        FROM Transactions WITH (NOLOCK)
        WHERE Id = @Id;
    ";

    /// <summary>
    /// Reference number ile işlem getir
    /// </summary>
    public const string GetByReferenceNumber = @"
        SELECT 
            Id,
            ReferenceNumber,
            TransactionTypeId,
            StatusId,
            Amount,
            Currency,
            AuthorizationCode,
            CardNumberMasked,
            MerchantId,
            MerchantCode,
            TerminalId,
            TerminalCode,
            DeclineReasonId,
            ErrorMessage,
            FraudCheckResultId,
            FraudScore,
            OriginalTransactionId,
            SettledAt,
            BatchNumber,
            CreatedAt
        FROM Transactions WITH (NOLOCK)
        WHERE ReferenceNumber = @ReferenceNumber;
    ";

    /// <summary>
    /// Kart bazlı günlük toplam (Limit hesaplama için kritik!)
    /// </summary>
    public const string GetDailyTotalByCard = @"
        SELECT ISNULL(SUM(Amount), 0)
        FROM Transactions WITH (NOLOCK)
        WHERE CardNumberMasked = @CardNumberMasked
          AND CreatedAt >= @StartOfDay
          AND CreatedAt < @EndOfDay
          AND StatusId IN (2, 7)  -- Approved, Settled
          AND TransactionTypeId IN (1, 4, 6);  -- Sale, PreAuth, CashAdvance (DecreasesLimit)
    ";

    /// <summary>
    /// Kart bazlı aylık toplam
    /// </summary>
    public const string GetMonthlyTotalByCard = @"
        SELECT ISNULL(SUM(Amount), 0)
        FROM Transactions WITH (NOLOCK)
        WHERE CardNumberMasked = @CardNumberMasked
          AND CreatedAt >= @StartOfMonth
          AND CreatedAt < @EndOfMonth
          AND StatusId IN (2, 7)  -- Approved, Settled
          AND TransactionTypeId IN (1, 4, 6);  -- Sale, PreAuth, CashAdvance
    ";

    /// <summary>
    /// Pending settlement işlemleri (takas için)
    /// </summary>
    public const string GetPendingSettlement = @"
        SELECT 
            Id,
            ReferenceNumber,
            TransactionTypeId,
            StatusId,
            Amount,
            Currency,
            AuthorizationCode,
            CardNumberMasked,
            MerchantId,
            MerchantCode,
            TerminalId,
            TerminalCode,
            CreatedAt
        FROM Transactions WITH (NOLOCK)
        WHERE StatusId = 2  -- Approved
        ORDER BY CreatedAt ASC;
    ";

    /// <summary>
    /// Merchant bazlı son N işlem
    /// </summary>
    public const string GetRecentByMerchant = @"
        SELECT TOP (@Limit)
            Id,
            ReferenceNumber,
            TransactionTypeId,
            StatusId,
            Amount,
            Currency,
            AuthorizationCode,
            CardNumberMasked,
            MerchantId,
            MerchantCode,
            TerminalId,
            TerminalCode,
            CreatedAt
        FROM Transactions WITH (NOLOCK)
        WHERE MerchantId = @MerchantId
        ORDER BY CreatedAt DESC;
    ";
}