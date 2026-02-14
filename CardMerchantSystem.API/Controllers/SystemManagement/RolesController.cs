using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Services;
using CardMerchantSystem.Shared.Kernel.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers.SystemManagement;

[Authorize(Policy = Policies.AdminOnly)]
public class RolesController : ApiControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Tüm rolleri listeler
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllRolesAsync(cancellationToken);
        return Ok(roles);
    }

    /// <summary>
    /// Sistem rollerini listeler (enum'dan)
    /// </summary>
    [HttpGet("system")]
    public async Task<ActionResult<List<RoleDto>>> GetSystemRoles()
    {
        var roles = await _roleService.GetSystemRolesAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Rol detayını getirir
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoleDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetRoleByIdAsync(id, cancellationToken);
        return Ok(HandleNotFound(role, "Rol", id));
    }

    /// <summary>
    /// Yeni rol oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var role = await _roleService.CreateRoleAsync(dto, cancellationToken);

        if (role == null)
            throw new ConflictException("Rol oluşturulamadı. Bu isimde rol zaten mevcut.");

        return CreatedResponse(nameof(GetById), new { id = role.Id }, role);
    }

    /// <summary>
    /// Rol bilgilerini günceller
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RoleDto>> Update(int id, [FromBody] UpdateRoleDto dto, CancellationToken cancellationToken)
    {
        var role = await _roleService.UpdateRoleAsync(id, dto, cancellationToken);
        return Ok(HandleNotFound(role, "Rol", id));
    }

    /// <summary>
    /// Rolü siler
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _roleService.DeleteRoleAsync(id, cancellationToken);

        if (!result)
            throw new BusinessRuleException("Rol silinemedi. Sistem rolleri veya kullanıcısı olan roller silinemez.");

        return (ActionResult)NoContent();
    }
}