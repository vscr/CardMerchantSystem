using CardMerchantSystem.Shared.Kernel;

namespace Card.Application.DTOs;

public class CardApplicationFilterDto : PagedRequest
{
    public int? StatusId { get; set; }
    public int? CardTypeId { get; set; }
    public string? CustomerTckn { get; set; }
    public string? CustomerName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}