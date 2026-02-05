using CardMerchantSystem.Shared.Kernel;
using CardMerchantSystem.Shared.Kernel.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CardMerchantSystem.API.Controllers;

/// <summary>
/// Tüm API controller'ları için base class.
/// Standart response handling, Result pattern desteği ve yardımcı metodlar sağlar.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    #region Result Handling

    /// <summary>
    /// Result başarısızsa BusinessRuleException fırlatır, başarılıysa değeri döner
    /// </summary>
    protected T HandleResult<T>(Result<T> result)
    {
        if (result.IsFailure)
            throw new BusinessRuleException(result.Error!, result.ErrorCode ?? "BUSINESS_ERROR");

        return result.Value!;
    }

    /// <summary>
    /// Result başarısızsa BusinessRuleException fırlatır
    /// </summary>
    protected void HandleResult(Result result)
    {
        if (result.IsFailure)
            throw new BusinessRuleException(result.Error!, result.ErrorCode ?? "BUSINESS_ERROR");
    }

    /// <summary>
    /// Result'ı ActionResult'a çevirir (Ok veya BadRequest)
    /// Exception fırlatmak istemiyorsanız bu metodu kullanın
    /// </summary>
    protected ActionResult<T> ToActionResult<T>(Result<T> result)
    {
        if (result.IsFailure)
            return BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));

        return Ok(result.Value);
    }

    /// <summary>
    /// Result'ı IActionResult'a çevirir (Ok veya BadRequest)
    /// </summary>
    protected IActionResult ToActionResult(Result result, string? successMessage = null)
    {
        if (result.IsFailure)
            return BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));

        return successMessage != null
            ? Ok(new { message = successMessage })
            : Ok();
    }

    #endregion

    #region Not Found Handling

    /// <summary>
    /// Entity null ise NotFoundException fırlatır
    /// </summary>
    protected T HandleNotFound<T>(T? entity, string entityName, object? id = null)
    {
        if (entity is null)
            throw new NotFoundException(entityName, id);

        return entity;
    }

    /// <summary>
    /// Entity null ise NotFound döner, değilse Ok döner
    /// </summary>
    protected ActionResult<T> OkOrNotFound<T>(T? entity, string entityName, object? id = null)
    {
        if (entity is null)
            return NotFound(new ApiErrorResponse($"{entityName} bulunamadı{(id != null ? $": {id}" : "")}", "NOT_FOUND"));

        return Ok(entity);
    }

    #endregion

    #region Created Responses

    /// <summary>
    /// CreatedAtAction response döner
    /// </summary>
    protected CreatedAtActionResult CreatedResponse<T>(string actionName, object routeValues, T value)
    {
        return CreatedAtAction(actionName, routeValues, value);
    }

    /// <summary>
    /// Result başarılıysa CreatedAtAction döner, değilse BadRequest
    /// </summary>
    protected ActionResult<T> CreatedOrBadRequest<T>(
        Result<T> result,
        string actionName,
        Func<T, object> routeValuesFactory)
    {
        if (result.IsFailure)
            return BadRequest(new ApiErrorResponse(result.Error!, result.ErrorCode));

        return CreatedAtAction(actionName, routeValuesFactory(result.Value!), result.Value);
    }

    #endregion

    #region Success Responses

    /// <summary>
    /// Başarılı mesaj ile Ok döner
    /// </summary>
    protected IActionResult OkWithMessage(string message)
    {
        return Ok(new { message });
    }

    /// <summary>
    /// NoContent (204) döner - Update/Delete işlemleri için
    /// </summary>
    protected new IActionResult NoContent()
    {
        return base.NoContent();
    }

    #endregion

    #region User Context

    /// <summary>
    /// Mevcut kullanıcının ID'sini döner
    /// </summary>
    protected Guid? CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Mevcut kullanıcının kullanıcı adını döner
    /// </summary>
    protected string? CurrentUsername => User.FindFirst(ClaimTypes.Name)?.Value
                                      ?? User.FindFirst("username")?.Value
                                      ?? User.Identity?.Name;

    /// <summary>
    /// Mevcut kullanıcının rollerini döner
    /// </summary>
    protected IEnumerable<string> CurrentUserRoles => User.FindAll(ClaimTypes.Role).Select(c => c.Value);

    /// <summary>
    /// Kullanıcının belirli bir rolde olup olmadığını kontrol eder
    /// </summary>
    protected bool IsInRole(string role) => User.IsInRole(role);

    #endregion

    #region Request Context

    /// <summary>
    /// Request'teki Correlation ID'yi döner
    /// </summary>
    protected string? CorrelationId => HttpContext.Items.TryGetValue("CorrelationId", out var value)
        ? value?.ToString()
        : HttpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault();

    /// <summary>
    /// Client IP adresini döner
    /// </summary>
    protected string? ClientIpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();

    #endregion
}

/// <summary>
/// Standart API hata response'u
/// </summary>
public class ApiErrorResponse
{
    public string Error { get; }
    public string? Code { get; }
    public DateTime Timestamp { get; }

    public ApiErrorResponse(string error, string? code = null)
    {
        Error = error;
        Code = code;
        Timestamp = DateTime.UtcNow;
    }
}