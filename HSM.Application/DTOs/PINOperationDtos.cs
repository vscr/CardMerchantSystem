namespace HSM.Application.DTOs;

/// <summary>
/// PIN Block oluşturma request
/// </summary>
public class GeneratePINBlockRequestDto
{
    public string CardNumber { get; set; } = null!;
    public string PIN { get; set; } = null!;
    public string? PINBlockFormat { get; set; } = "01";
}

/// <summary>
/// PIN Block oluşturma response
/// </summary>
public class GeneratePINBlockResponseDto
{
    public bool IsSuccess { get; set; }
    public string? PINBlock { get; set; }
    public string? PINBlockFormat { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// PIN doğrulama request
/// </summary>
public class VerifyPINRequestDto
{
    public string CardNumber { get; set; } = null!;
    public string PINBlock { get; set; } = null!;
    public string? PINBlockFormat { get; set; } = "01";
}

/// <summary>
/// PIN doğrulama response
/// </summary>
public class VerifyPINResponseDto
{
    public bool IsSuccess { get; set; }
    public bool IsValid { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// PIN translate request
/// </summary>
public class TranslatePINBlockRequestDto
{
    public string CardNumber { get; set; } = null!;
    public string SourcePINBlock { get; set; } = null!;
    public string SourcePINBlockFormat { get; set; } = "01";
    public string DestinationPINBlockFormat { get; set; } = "01";
}

/// <summary>
/// PIN translate response
/// </summary>
public class TranslatePINBlockResponseDto
{
    public bool IsSuccess { get; set; }
    public string? TranslatedPINBlock { get; set; }
    public string? PINBlockFormat { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}