using Merchant.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardMerchantSystem.API.Controllers.Merchant;

[Route("api/merchant/dapper-test")]
[Authorize]
public class MerchantDapperTestController : ApiControllerBase
{
    private readonly IMerchantDapperRepository _dapperRepository;

    public MerchantDapperTestController(IMerchantDapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }

    /// <summary>
    /// Dapper test - Get by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var merchant = await _dapperRepository.GetByIdAsync(id, ct);
        return merchant == null ? NotFound() : Ok(merchant);
    }

    /// <summary>
    /// Dapper test - Get all merchants
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var merchants = await _dapperRepository.GetAllAsync(ct);
        return Ok(new
        {
            Count = merchants.Count(),
            Data = merchants
        });
    }

    /// <summary>
    /// Dapper test - Get by tax number
    /// </summary>
    [HttpGet("by-tax/{taxNumber}")]
    public async Task<IActionResult> GetByTaxNumber(string taxNumber, CancellationToken ct)
    {
        var merchant = await _dapperRepository.GetByTaxNumberAsync(taxNumber, ct);
        return merchant == null ? NotFound() : Ok(merchant);
    }

    /// <summary>
    /// Dapper test - Get active count
    /// </summary>
    [HttpGet("active-count")]
    public async Task<IActionResult> GetActiveCount(CancellationToken ct)
    {
        var count = await _dapperRepository.GetActiveMerchantCountAsync(ct);
        return Ok(new { ActiveCount = count, Message = "StatusId = 3 olan merchantlar" });
    }

    /// <summary>
    /// Dapper test - Get paged
    /// </summary>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        CancellationToken ct = default)
    {
        var result = await _dapperRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, ct);

        return Ok(new
        {
            Data = result.Data,
            TotalCount = result.TotalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize),
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < (int)Math.Ceiling((double)result.TotalCount / pageSize)
        });
    }

    /// <summary>
    /// Dapper test - Get merchant detail with terminals and transactions
    /// </summary>
    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct)
    {
        var detail = await _dapperRepository.GetMerchantDetailAsync(id, ct);
        return detail == null ? NotFound() : Ok(detail);
    }
}