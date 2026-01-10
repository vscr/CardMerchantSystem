using CardMerchantSystem.Shared.Kernel;

namespace HSM.Domain.Services;

/// <summary>
/// HSM Servis Interface
/// </summary>
public interface IHSMService
{
    // PIN İşlemleri
    Task<Result<GeneratePINBlockResponse>> GeneratePINBlockAsync(GeneratePINBlockRequest request, CancellationToken cancellationToken = default);
    Task<Result<VerifyPINResponse>> VerifyPINAsync(VerifyPINRequest request, CancellationToken cancellationToken = default);
    Task<Result<TranslatePINBlockResponse>> TranslatePINBlockAsync(TranslatePINBlockRequest request, CancellationToken cancellationToken = default);
    Task<Result<GenerateRandomPINResponse>> GenerateRandomPINAsync(GenerateRandomPINRequest request, CancellationToken cancellationToken = default);

    // CVV İşlemleri
    Task<Result<GenerateCVVResponse>> GenerateCVVAsync(GenerateCVVRequest request, CancellationToken cancellationToken = default);
    Task<Result<VerifyCVVResponse>> VerifyCVVAsync(VerifyCVVRequest request, CancellationToken cancellationToken = default);
    Task<Result<GenerateCVV2Response>> GenerateCVV2Async(GenerateCVV2Request request, CancellationToken cancellationToken = default);
    Task<Result<VerifyCVV2Response>> VerifyCVV2Async(VerifyCVV2Request request, CancellationToken cancellationToken = default);

    // Key Yönetimi
    Task<Result<GenerateKeyResponse>> GenerateKeyAsync(GenerateKeyRequest request, CancellationToken cancellationToken = default);
    Task<Result<ImportKeyResponse>> ImportKeyAsync(ImportKeyRequest request, CancellationToken cancellationToken = default);

    // Şifreleme
    Task<Result<EncryptDataResponse>> EncryptDataAsync(EncryptDataRequest request, CancellationToken cancellationToken = default);
    Task<Result<DecryptDataResponse>> DecryptDataAsync(DecryptDataRequest request, CancellationToken cancellationToken = default);

    // MAC İşlemleri
    Task<Result<GenerateMACResponse>> GenerateMACAsync(GenerateMACRequest request, CancellationToken cancellationToken = default);
    Task<Result<VerifyMACResponse>> VerifyMACAsync(VerifyMACRequest request, CancellationToken cancellationToken = default);

    // ARQC/ARPC (EMV)
    Task<Result<VerifyARQCResponse>> VerifyARQCAsync(VerifyARQCRequest request, CancellationToken cancellationToken = default);

    // Diagnostik
    Task<Result<HealthCheckResponse>> HealthCheckAsync(CancellationToken cancellationToken = default);
}

#region PIN Request/Response Models

public class GeneratePINBlockRequest
{
    public string CardNumber { get; set; } = null!;
    public string PIN { get; set; } = null!;
    public string PINBlockFormat { get; set; } = "01"; // ISO Format 0
    public string ZPKIndex { get; set; } = null!;
}

public class GeneratePINBlockResponse
{
    public string PINBlock { get; set; } = null!;
    public string PINBlockFormat { get; set; } = null!;
}

public class VerifyPINRequest
{
    public string CardNumber { get; set; } = null!;
    public string PINBlock { get; set; } = null!;
    public string PINBlockFormat { get; set; } = "01";
    public string ZPKIndex { get; set; } = null!;
    public string PVKIndex { get; set; } = null!;
    public string PINValidationData { get; set; } = null!;
    public int PINLength { get; set; } = 4;
}

public class VerifyPINResponse
{
    public bool IsValid { get; set; }
    public string ResponseCode { get; set; } = null!;
}

public class TranslatePINBlockRequest
{
    public string CardNumber { get; set; } = null!;
    public string SourcePINBlock { get; set; } = null!;
    public string SourcePINBlockFormat { get; set; } = "01";
    public string DestinationPINBlockFormat { get; set; } = "01";
    public string SourceZPKIndex { get; set; } = null!;
    public string DestinationZPKIndex { get; set; } = null!;
}

public class TranslatePINBlockResponse
{
    public string TranslatedPINBlock { get; set; } = null!;
    public string PINBlockFormat { get; set; } = null!;
}

public class GenerateRandomPINRequest
{
    public string CardNumber { get; set; } = null!;
    public int PINLength { get; set; } = 4;
    public string PVKIndex { get; set; } = null!;
}

public class GenerateRandomPINResponse
{
    public string EncryptedPIN { get; set; } = null!;
    public string PINOffset { get; set; } = null!;
}

#endregion

#region CVV Request/Response Models

public class GenerateCVVRequest
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string ServiceCode { get; set; } = "101";
    public string CVKIndex { get; set; } = null!;
}

public class GenerateCVVResponse
{
    public string CVV { get; set; } = null!;
}

public class VerifyCVVRequest
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string ServiceCode { get; set; } = "101";
    public string CVV { get; set; } = null!;
    public string CVKIndex { get; set; } = null!;
}

public class VerifyCVVResponse
{
    public bool IsValid { get; set; }
    public string ResponseCode { get; set; } = null!;
}

public class GenerateCVV2Request
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string CVKIndex { get; set; } = null!;
}

public class GenerateCVV2Response
{
    public string CVV2 { get; set; } = null!;
}

public class VerifyCVV2Request
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string CVV2 { get; set; } = null!;
    public string CVKIndex { get; set; } = null!;
}

public class VerifyCVV2Response
{
    public bool IsValid { get; set; }
    public string ResponseCode { get; set; } = null!;
}

#endregion

#region Key Management Request/Response Models

public class GenerateKeyRequest
{
    public string KeyType { get; set; } = null!;
    public int KeyLength { get; set; } = 32; // Double length
    public string? ZMKIndex { get; set; }
    public string KeyName { get; set; } = null!;
}

public class GenerateKeyResponse
{
    public string EncryptedKey { get; set; } = null!;
    public string KeyCheckValue { get; set; } = null!;
    public string KeyIndex { get; set; } = null!;
}

public class ImportKeyRequest
{
    public string KeyType { get; set; } = null!;
    public string EncryptedKey { get; set; } = null!;
    public string ZMKIndex { get; set; } = null!;
    public string KeyName { get; set; } = null!;
}

public class ImportKeyResponse
{
    public string LocalEncryptedKey { get; set; } = null!;
    public string KeyCheckValue { get; set; } = null!;
    public string KeyIndex { get; set; } = null!;
}

#endregion

#region Encryption Request/Response Models

public class EncryptDataRequest
{
    public string PlainData { get; set; } = null!;
    public string DEKIndex { get; set; } = null!;
    public string? InitializationVector { get; set; }
}

public class EncryptDataResponse
{
    public string EncryptedData { get; set; } = null!;
    public string? InitializationVector { get; set; }
}

public class DecryptDataRequest
{
    public string EncryptedData { get; set; } = null!;
    public string DEKIndex { get; set; } = null!;
    public string? InitializationVector { get; set; }
}

public class DecryptDataResponse
{
    public string PlainData { get; set; } = null!;
}

#endregion

#region MAC Request/Response Models

public class GenerateMACRequest
{
    public string Data { get; set; } = null!;
    public string MACKeyIndex { get; set; } = null!;
    public string MACAlgorithm { get; set; } = "01"; // ISO 9797-1 Algorithm 1
}

public class GenerateMACResponse
{
    public string MAC { get; set; } = null!;
}

public class VerifyMACRequest
{
    public string Data { get; set; } = null!;
    public string MAC { get; set; } = null!;
    public string MACKeyIndex { get; set; } = null!;
    public string MACAlgorithm { get; set; } = "01";
}

public class VerifyMACResponse
{
    public bool IsValid { get; set; }
    public string ResponseCode { get; set; } = null!;
}

#endregion

#region EMV Request/Response Models

public class VerifyARQCRequest
{
    public string CardNumber { get; set; } = null!;
    public string ARQC { get; set; } = null!;
    public string TransactionData { get; set; } = null!;
    public string MDKIndex { get; set; } = null!;
    public string ATC { get; set; } = null!;
    public string UN { get; set; } = null!;
}

public class VerifyARQCResponse
{
    public bool IsValid { get; set; }
    public string? ARPC { get; set; }
    public string ResponseCode { get; set; } = null!;
}

#endregion

#region Diagnostics

public class HealthCheckResponse
{
    public bool IsHealthy { get; set; }
    public string DeviceName { get; set; } = null!;
    public string FirmwareVersion { get; set; } = null!;
    public int ResponseTimeMs { get; set; }
    public string Status { get; set; } = null!;
}

#endregion