using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Services;
using CardMerchantSystem.Shared.Kernel.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

[Authorize(Policy = Policies.AdminOnly)]
public class UsersController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Tüm kullanıcıları listeler
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Kullanıcı detayını getirir
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        return Ok(HandleNotFound(user, "Kullanıcı", id));
    }

    /// <summary>
    /// Yeni kullanıcı oluşturur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        var createdBy = User.Identity?.Name ?? "System";
        var user = await _userService.CreateUserAsync(dto, createdBy, cancellationToken);

        if (user == null)
            throw new ConflictException("Kullanıcı oluşturulamadı. Kullanıcı adı veya email zaten mevcut.");

        return CreatedResponse(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Kullanıcı bilgilerini günceller
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var updatedBy = User.Identity?.Name ?? "System";
        var user = await _userService.UpdateUserAsync(id, dto, updatedBy, cancellationToken);

        if (user == null)
            throw new ConflictException("Kullanıcı güncellenemedi. Email zaten kullanımda olabilir.");

        return Ok(user);
    }

    /// <summary>
    /// Kullanıcı şifresini değiştirir
    /// </summary>
    [HttpPut("{id:guid}/password")]
    public async Task<ActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        var updatedBy = User.Identity?.Name ?? "System";
        var result = await _userService.ChangePasswordAsync(id, dto, updatedBy, cancellationToken);

        if (!result)
            throw new NotFoundException("Kullanıcı", id);

        return Ok(new { message = "Şifre başarıyla değiştirildi" });
    }

    /// <summary>
    /// Kullanıcı rollerini günceller
    /// </summary>
    [HttpPut("{id:guid}/roles")]
    public async Task<ActionResult<UserDto>> UpdateRoles(Guid id, [FromBody] UpdateUserRolesDto dto, CancellationToken cancellationToken)
    {
        var updatedBy = User.Identity?.Name ?? "System";
        var user = await _userService.UpdateUserRolesAsync(id, dto, updatedBy, cancellationToken);

        return Ok(HandleNotFound(user, "Kullanıcı", id));
    }

    /// <summary>
    /// Kullanıcı durumunu değiştirir (aktif/pasif)
    /// </summary>
    [HttpPut("{id:guid}/toggle-status")]
    public async Task<ActionResult> ToggleStatus(Guid id, CancellationToken cancellationToken)
    {
        var updatedBy = User.Identity?.Name ?? "System";
        var result = await _userService.ToggleUserStatusAsync(id, updatedBy, cancellationToken);

        if (!result)
            throw new BusinessRuleException("Kullanıcı durumu değiştirilemedi. Admin kullanıcısı deaktif edilemez.");

        return Ok(new { message = "Kullanıcı durumu değiştirildi" });
    }

    /// <summary>
    /// Kullanıcıyı siler
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUserAsync(id, cancellationToken);

        if (!result)
            throw new BusinessRuleException("Kullanıcı silinemedi. Admin kullanıcısı silinemez.");

        return NoContent();
    }
}