using Card.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Queries;

/// <summary>
/// Başvuru ve durum geçmişini getirme sorgusu
/// </summary>
public record GetApplicationWithHistoryQuery(Guid Id) : IRequest<Result<ApplicationWithHistoryDto>>;

/// <summary>
/// Başvuru + Durum geçmişi DTO
/// </summary>
public class ApplicationWithHistoryDto
{
    public CardApplicationDto Application { get; set; } = null!;
    public List<StatusHistoryDto> StatusHistory { get; set; } = new();
}