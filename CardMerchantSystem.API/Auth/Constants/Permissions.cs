namespace CardMerchantSystem.API.Auth.Constants;

/// <summary>
/// Policy isimleri
/// </summary>
public static class Policies
{
    // Genel
    public const string AdminOnly = "AdminOnly";
    public const string ViewerOrAbove = "ViewerOrAbove";

    // Modül bazlı
    public const string CardManagement = "CardManagement";
    public const string MerchantManagement = "MerchantManagement";
    public const string FinanceManagement = "FinanceManagement";
    public const string ComplianceManagement = "ComplianceManagement";
    public const string CallCenterAccess = "CallCenterAccess";
    public const string WorkOrderManagement = "WorkOrderManagement";
    public const string FraudManagement = "FraudManagement";
}

/// <summary>
/// Rol isimleri (Authorize attribute için)
/// </summary>
public static class RoleNames
{
    public const string Admin = "Admin";
    public const string CardOperator = "CardOperator";
    public const string MerchantOperator = "MerchantOperator";
    public const string FinanceOperator = "FinanceOperator";
    public const string ComplianceOfficer = "ComplianceOfficer";
    public const string CallCenterAgent = "CallCenterAgent";
    public const string Viewer = "Viewer";

    // Kombinasyonlar
    public const string AdminOrCardOperator = $"{Admin},{CardOperator}";
    public const string AdminOrMerchantOperator = $"{Admin},{MerchantOperator}";
    public const string AdminOrFinanceOperator = $"{Admin},{FinanceOperator}";
    public const string AdminOrComplianceOfficer = $"{Admin},{ComplianceOfficer}";
    public const string CardTeam = $"{Admin},{CardOperator},{CallCenterAgent}";
    public const string FinanceTeam = $"{Admin},{FinanceOperator},{ComplianceOfficer}";
    public const string FraudTeam = $"{Admin},{ComplianceOfficer},{CardOperator}";
}