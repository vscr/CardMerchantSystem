using BKM.Domain.Entities;
using CardMerchantSystem.Shared.Kernel;

namespace BKM.Domain.Services;

/// <summary>
/// Switch servisi interface
/// </summary>
public interface ISwitchService
{
    /// <summary>
    /// Authorization işlemi
    /// </summary>
    Task<AuthorizationResponse> ProcessAuthorizationAsync(AuthorizationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Financial işlemi (Sale)
    /// </summary>
    Task<FinancialResponse> ProcessFinancialAsync(FinancialRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reversal işlemi
    /// </summary>
    Task<ReversalResponse> ProcessReversalAsync(ReversalRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// BIN sorgulama
    /// </summary>
    Task<BINInfo?> GetBINInfoAsync(string bin, CancellationToken cancellationToken = default);
}

/// <summary>
/// Authorization Request
/// </summary>
public class AuthorizationRequest
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string CVV { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string TerminalId { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string MCC { get; set; } = null!;
    public string AcquirerBankCode { get; set; } = null!;
}

/// <summary>
/// Authorization Response
/// </summary>
public class AuthorizationResponse
{
    public bool IsApproved { get; set; }
    public string ResponseCode { get; set; } = null!;
    public string ResponseMessage { get; set; } = null!;
    public string? AuthorizationCode { get; set; }
    public string STAN { get; set; } = null!;
    public string RRN { get; set; } = null!;
    public int ProcessingTimeMs { get; set; }
}

/// <summary>
/// Financial Request
/// </summary>
public class FinancialRequest
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string CVV { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "TRY";
    public string TerminalId { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string MCC { get; set; } = null!;
    public string AcquirerBankCode { get; set; } = null!;
    public string? OriginalSTAN { get; set; }
    public string? OriginalAuthCode { get; set; }
}

/// <summary>
/// Financial Response
/// </summary>
public class FinancialResponse
{
    public bool IsApproved { get; set; }
    public string ResponseCode { get; set; } = null!;
    public string ResponseMessage { get; set; } = null!;
    public string? AuthorizationCode { get; set; }
    public string STAN { get; set; } = null!;
    public string RRN { get; set; } = null!;
    public int ProcessingTimeMs { get; set; }
    public Guid? ClearingRecordId { get; set; }
}

/// <summary>
/// Reversal Request
/// </summary>
public class ReversalRequest
{
    public string OriginalSTAN { get; set; } = null!;
    public string OriginalRRN { get; set; } = null!;
    public string OriginalAuthCode { get; set; } = null!;
    public decimal Amount { get; set; }
    public string TerminalId { get; set; } = null!;
    public string MerchantId { get; set; } = null!;
    public string AcquirerBankCode { get; set; } = null!;
    public string ReversalReason { get; set; } = null!;
}

/// <summary>
/// Reversal Response
/// </summary>
public class ReversalResponse
{
    public bool IsApproved { get; set; }
    public string ResponseCode { get; set; } = null!;
    public string ResponseMessage { get; set; } = null!;
    public string STAN { get; set; } = null!;
    public string RRN { get; set; } = null!;
    public int ProcessingTimeMs { get; set; }
}

/// <summary>
/// BIN Bilgisi
/// </summary>
public class BINInfo
{
    public string BIN { get; set; } = null!;
    public string BankCode { get; set; } = null!;
    public string BankName { get; set; } = null!;
    public string CardBrand { get; set; } = null!;
    public string CardType { get; set; } = null!;
    public string CardLevel { get; set; } = null!;
}