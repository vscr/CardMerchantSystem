namespace CardMerchantSystem.Shared.Kernel;

/// <summary>
/// Yaygın hata kodları.
/// API response ve loglama için standart kodlar.
/// </summary>
public static class ErrorCodes
{
    // Card Module
    public const string CardApplicationNotFound = "CARD_APP_NOT_FOUND";
    public const string CardApplicationAlreadyExists = "CARD_APP_ALREADY_EXISTS";
    public const string CardApplicationInvalidStatus = "CARD_APP_INVALID_STATUS";
    public const string CardNotFound = "CARD_NOT_FOUND";
    public const string CardAlreadyActive = "CARD_ALREADY_ACTIVE";
    public const string CardBlocked = "CARD_BLOCKED";

    // Limit Module
    public const string LimitExceeded = "LIMIT_EXCEEDED";
    public const string LimitNotFound = "LIMIT_NOT_FOUND";
    public const string InsufficientLimit = "INSUFFICIENT_LIMIT";

    // Merchant Module
    public const string MerchantNotFound = "MERCHANT_NOT_FOUND";
    public const string MerchantNotActive = "MERCHANT_NOT_ACTIVE";
    public const string TerminalNotFound = "TERMINAL_NOT_FOUND";

    // Transaction Module
    public const string TransactionFailed = "TRANSACTION_FAILED";
    public const string FraudDetected = "FRAUD_DETECTED";
    public const string RefundAlreadyProcessed = "REFUND_ALREADY_PROCESSED";
    public const string RefundAmountExceeded = "REFUND_AMOUNT_EXCEEDED";
    public const string TransactionNotRefundable = "TRANSACTION_NOT_REFUNDABLE";

    // Dispute Module
    public const string DisputeNotFound = "DISPUTE_NOT_FOUND";
    public const string DisputeInvalidStatus = "DISPUTE_INVALID_STATUS";
    public const string DisputeAlreadyResolved = "DISPUTE_ALREADY_RESOLVED";

    // General
    public const string NotFound = "NOT_FOUND";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string ConcurrencyError = "CONCURRENCY_ERROR";
    public const string UnauthorizedAccess = "UNAUTHORIZED_ACCESS";
    public const string SystemError = "SYSTEM_ERROR";
}