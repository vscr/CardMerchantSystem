namespace HSM.Application.DTOs;

/// <summary>
/// CVV oluşturma request
/// </summary>
public class GenerateCVVRequestDto
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string ServiceCode { get; set; } = "101";
}

/// <summary>
/// CVV oluşturma response
/// </summary>
public class GenerateCVVResponseDto
{
    public bool IsSuccess { get; set; }
    public string? CVV { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// CVV doğrulama request
/// </summary>
public class VerifyCVVRequestDto
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string ServiceCode { get; set; } = "101";
    public string CVV { get; set; } = null!;
}

/// <summary>
/// CVV doğrulama response
/// </summary>
public class VerifyCVVResponseDto
{
    public bool IsSuccess { get; set; }
    public bool IsValid { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// CVV2 oluşturma request
/// </summary>
public class GenerateCVV2RequestDto
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
}

/// <summary>
/// CVV2 oluşturma response
/// </summary>
public class GenerateCVV2ResponseDto
{
    public bool IsSuccess { get; set; }
    public string? CVV2 { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// CVV2 doğrulama request
/// </summary>
public class VerifyCVV2RequestDto
{
    public string CardNumber { get; set; } = null!;
    public string ExpiryDate { get; set; } = null!;
    public string CVV2 { get; set; } = null!;
}

/// <summary>
/// CVV2 doğrulama response
/// </summary>
public class VerifyCVV2ResponseDto
{
    public bool IsSuccess { get; set; }
    public bool IsValid { get; set; }
    public int ExecutionTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}