using Merchant.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Queries;

public record GetMerchantWithTerminalsQuery(Guid Id) : IRequest<Result<MerchantWithTerminalsDto>>;

public class MerchantWithTerminalsDto
{
    public MerchantDto Merchant { get; set; } = null!;
    public List<TerminalDto> Terminals { get; set; } = new();
}