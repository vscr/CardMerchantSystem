using CardMerchantSystem.Shared.Kernel;
using CardMerchantSystem.Shared.Kernel.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers;

/// <summary>
/// Tüm API controller'ları için base class
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Result başarısızsa exception fırlatır
    /// </summary>
    protected T HandleResult<T>(Result<T> result)
    {
        if (result.IsFailure)
            throw new BusinessRuleException(result.Error);

        return result.Value!;
    }

    /// <summary>
    /// Result başarısızsa exception fırlatır
    /// </summary>
    protected void HandleResult(Result result)
    {
        if (result.IsFailure)
            throw new BusinessRuleException(result.Error);
    }

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
    /// Created response döner
    /// </summary>
    protected CreatedAtActionResult CreatedResponse<T>(string actionName, object routeValues, T value)
    {
        return CreatedAtAction(actionName, routeValues, value);
    }
}