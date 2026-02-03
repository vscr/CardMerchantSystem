namespace Transaction.Infrastructure.Dapper;

/// <summary>
/// Raporlama ve istatistik sorguları
/// </summary>
public static class ReportQueries
{
    /// <summary>
    /// Tarih aralığı istatistikleri (Dashboard için)
    /// </summary>
    public const string GetTransactionStats = @"
        SELECT 
            COUNT(*) AS TotalCount,
            ISNULL(SUM(Amount), 0) AS TotalAmount,
            
            -- Başarılı işlemler (Approved + Settled)
            SUM(CASE WHEN StatusId IN (2, 7) THEN 1 ELSE 0 END) AS SuccessfulCount,
            ISNULL(SUM(CASE WHEN StatusId IN (2, 7) THEN Amount ELSE 0 END), 0) AS SuccessfulAmount,
            
            -- Reddedilen işlemler
            SUM(CASE WHEN StatusId = 3 THEN 1 ELSE 0 END) AS DeclinedCount,
            ISNULL(SUM(CASE WHEN StatusId = 3 THEN Amount ELSE 0 END), 0) AS DeclinedAmount,
            
            -- İade işlemleri (TransactionType = Refund ve başarılı)
            SUM(CASE WHEN TransactionTypeId = 2 AND StatusId IN (2, 7) THEN 1 ELSE 0 END) AS RefundedCount,
            ISNULL(SUM(CASE WHEN TransactionTypeId = 2 AND StatusId IN (2, 7) THEN Amount ELSE 0 END), 0) AS RefundedAmount,
            
            -- İptal edilenler (Reversed)
            SUM(CASE WHEN StatusId = 6 THEN 1 ELSE 0 END) AS ReversedCount,
            ISNULL(SUM(CASE WHEN StatusId = 6 THEN Amount ELSE 0 END), 0) AS ReversedAmount,
            
            -- Bekleyenler
            SUM(CASE WHEN StatusId = 1 THEN 1 ELSE 0 END) AS PendingCount,
            
            -- Takas edilenler
            SUM(CASE WHEN StatusId = 7 THEN 1 ELSE 0 END) AS SettledCount,
            ISNULL(SUM(CASE WHEN StatusId = 7 THEN Amount ELSE 0 END), 0) AS SettledAmount
            
        FROM Transactions WITH (NOLOCK)
        WHERE CreatedAt >= @StartDate AND CreatedAt <= @EndDate
          AND (@MerchantId IS NULL OR MerchantId = @MerchantId);
    ";

    /// <summary>
    /// İşlem tipi bazlı dağılım
    /// </summary>
    public const string GetStatsByTransactionType = @"
        SELECT 
            TransactionTypeId,
            COUNT(*) AS Count,
            ISNULL(SUM(Amount), 0) AS Amount
        FROM Transactions WITH (NOLOCK)
        WHERE CreatedAt >= @StartDate AND CreatedAt <= @EndDate
          AND (@MerchantId IS NULL OR MerchantId = @MerchantId)
        GROUP BY TransactionTypeId
        ORDER BY Count DESC;
    ";

    /// <summary>
    /// Günlük işlem trendi (son N gün)
    /// </summary>
    public const string GetDailyTrend = @"
        SELECT 
            CAST(CreatedAt AS DATE) AS Date,
            COUNT(*) AS Count,
            ISNULL(SUM(Amount), 0) AS Amount,
            SUM(CASE WHEN StatusId IN (2, 7) THEN 1 ELSE 0 END) AS SuccessfulCount,
            SUM(CASE WHEN StatusId = 3 THEN 1 ELSE 0 END) AS DeclinedCount
        FROM Transactions WITH (NOLOCK)
        WHERE CreatedAt >= @StartDate AND CreatedAt <= @EndDate
          AND (@MerchantId IS NULL OR MerchantId = @MerchantId)
        GROUP BY CAST(CreatedAt AS DATE)
        ORDER BY Date ASC;
    ";

    /// <summary>
    /// Saatlik işlem dağılımı (bugün için)
    /// </summary>
    public const string GetHourlyDistribution = @"
        SELECT 
            DATEPART(HOUR, CreatedAt) AS Hour,
            COUNT(*) AS Count,
            ISNULL(SUM(Amount), 0) AS Amount
        FROM Transactions WITH (NOLOCK)
        WHERE CreatedAt >= @StartOfDay AND CreatedAt < @EndOfDay
          AND (@MerchantId IS NULL OR MerchantId = @MerchantId)
        GROUP BY DATEPART(HOUR, CreatedAt)
        ORDER BY Hour;
    ";

    /// <summary>
    /// Merchant bazlı özet (Top N)
    /// </summary>
    public const string GetTopMerchants = @"
        SELECT TOP (@Limit)
            MerchantId,
            MerchantCode,
            COUNT(*) AS TransactionCount,
            ISNULL(SUM(Amount), 0) AS TotalAmount,
            ISNULL(SUM(CASE WHEN StatusId IN (2, 7) THEN Amount ELSE 0 END), 0) AS SuccessfulAmount,
            CAST(SUM(CASE WHEN StatusId IN (2, 7) THEN 1.0 ELSE 0 END) / COUNT(*) * 100 AS DECIMAL(5,2)) AS SuccessRate
        FROM Transactions WITH (NOLOCK)
        WHERE CreatedAt >= @StartDate AND CreatedAt <= @EndDate
        GROUP BY MerchantId, MerchantCode
        ORDER BY TotalAmount DESC;
    ";

    /// <summary>
    /// Decline reason dağılımı
    /// </summary>
    public const string GetDeclineReasonStats = @"
        SELECT 
            DeclineReasonId,
            COUNT(*) AS Count,
            ISNULL(SUM(Amount), 0) AS Amount
        FROM Transactions WITH (NOLOCK)
        WHERE CreatedAt >= @StartDate AND CreatedAt <= @EndDate
          AND StatusId = 3  -- Declined
          AND DeclineReasonId IS NOT NULL
          AND (@MerchantId IS NULL OR MerchantId = @MerchantId)
        GROUP BY DeclineReasonId
        ORDER BY Count DESC;
    ";

    /// <summary>
    /// Settlement batch özeti
    /// </summary>
    public const string GetSettlementBatchSummary = @"
        SELECT 
            BatchNumber,
            COUNT(*) AS TransactionCount,
            ISNULL(SUM(Amount), 0) AS TotalAmount,
            MIN(CreatedAt) AS FirstTransaction,
            MAX(CreatedAt) AS LastTransaction,
            MAX(SettledAt) AS SettledAt
        FROM Transactions WITH (NOLOCK)
        WHERE BatchNumber IS NOT NULL
          AND (@BatchNumber IS NULL OR BatchNumber = @BatchNumber)
          AND (@StartDate IS NULL OR SettledAt >= @StartDate)
          AND (@EndDate IS NULL OR SettledAt <= @EndDate)
        GROUP BY BatchNumber
        ORDER BY SettledAt DESC;
    ";

    /// <summary>
    /// Fraud detection özeti
    /// </summary>
    public const string GetFraudStats = @"
        SELECT 
            FraudCheckResultId,
            COUNT(*) AS Count,
            ISNULL(SUM(Amount), 0) AS Amount,
            AVG(CAST(FraudScore AS DECIMAL(5,2))) AS AvgFraudScore,
            MAX(FraudScore) AS MaxFraudScore
        FROM Transactions WITH (NOLOCK)
        WHERE CreatedAt >= @StartDate AND CreatedAt <= @EndDate
          AND FraudCheckResultId IS NOT NULL
          AND (@MerchantId IS NULL OR MerchantId = @MerchantId)
        GROUP BY FraudCheckResultId
        ORDER BY Count DESC;
    ";
}