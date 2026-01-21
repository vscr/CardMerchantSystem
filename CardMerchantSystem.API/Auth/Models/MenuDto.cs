namespace CardMerchantSystem.API.Auth.Models;

public class MenuDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
    public List<MenuClaimDto> Claims { get; set; } = new();
    public List<MenuDto> Children { get; set; } = new();
}

public class MenuClaimDto
{
    public Guid Id { get; set; }
    public string ClaimType { get; set; } = null!;
    public string ClaimValue { get; set; } = null!;
}

public class CreateMenuDto
{
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public List<string> Policies { get; set; } = new();
}

public class UpdateMenuDto
{
    public string Title { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsVisible { get; set; }
}

public class AddMenuClaimDto
{
    public string ClaimType { get; set; } = null!;  // "Role" veya "Policy"
    public string ClaimValue { get; set; } = null!;
}