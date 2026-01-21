using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Services;
using CardMerchantSystem.Shared.Kernel.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

public class MenusController : ApiControllerBase
{
    private readonly IMenuService _menuService;
    private readonly IAuthService _authService;

    public MenusController(IMenuService menuService, IAuthService authService)
    {
        _menuService = menuService;
        _authService = authService;
    }

    /// <summary>
    /// Kullanıcının erişebildiği menüleri ağaç yapısında getirir
    /// </summary>
    [HttpGet("my-menus")]
    [Authorize]
    public async Task<ActionResult<List<MenuDto>>> GetMyMenus(CancellationToken cancellationToken)
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            throw new UnauthorizedException();

        var user = await _authService.GetUserByUsernameAsync(username);
        if (user == null)
            throw new NotFoundException("Kullanıcı", username);

        var menus = await _menuService.GetMenusByUserAsync(user, cancellationToken);
        return Ok(menus);
    }

    /// <summary>
    /// Tüm menüleri listeler (Admin)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<List<MenuDto>>> GetAll(CancellationToken cancellationToken)
    {
        var menus = await _menuService.GetAllMenusAsync(cancellationToken);
        return Ok(menus);
    }

    /// <summary>
    /// Menü ağacını getirir (Admin)
    /// </summary>
    [HttpGet("tree")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<List<MenuDto>>> GetTree(CancellationToken cancellationToken)
    {
        var menus = await _menuService.GetMenuTreeAsync(cancellationToken);
        return Ok(menus);
    }

    /// <summary>
    /// Menü detayını getirir (Admin)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<MenuDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var menu = await _menuService.GetMenuByIdAsync(id, cancellationToken);
        return Ok(HandleNotFound(menu, "Menü", id));
    }

    /// <summary>
    /// Yeni menü oluşturur (Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<MenuDto>> Create([FromBody] CreateMenuDto dto, CancellationToken cancellationToken)
    {
        var menu = await _menuService.CreateMenuAsync(dto, cancellationToken);
        return CreatedResponse(nameof(GetById), new { id = menu.Id }, menu);
    }

    /// <summary>
    /// Menü günceller (Admin)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<MenuDto>> Update(Guid id, [FromBody] UpdateMenuDto dto, CancellationToken cancellationToken)
    {
        var menu = await _menuService.UpdateMenuAsync(id, dto, cancellationToken);
        return Ok(HandleNotFound(menu, "Menü", id));
    }

    /// <summary>
    /// Menü siler (Admin)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _menuService.DeleteMenuAsync(id, cancellationToken);
        if (!result)
            throw new BusinessRuleException("Menü silinemedi. Alt menüleri olan menüler silinemez.");

        return NoContent();
    }

    /// <summary>
    /// Menüye claim ekler (Admin)
    /// </summary>
    [HttpPost("{id:guid}/claims")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult<MenuDto>> AddClaim(Guid id, [FromBody] AddMenuClaimDto dto, CancellationToken cancellationToken)
    {
        var menu = await _menuService.AddClaimAsync(id, dto, cancellationToken);
        return Ok(HandleNotFound(menu, "Menü", id));
    }

    /// <summary>
    /// Menüden claim kaldırır (Admin)
    /// </summary>
    [HttpDelete("{menuId:guid}/claims/{claimId:guid}")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> RemoveClaim(Guid menuId, Guid claimId, CancellationToken cancellationToken)
    {
        var result = await _menuService.RemoveClaimAsync(menuId, claimId, cancellationToken);
        if (!result)
            throw new NotFoundException("Claim", claimId);

        return NoContent();
    }
}