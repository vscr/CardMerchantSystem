using Card.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Queries;

/// <summary>
/// ID ile başvuru getirme sorgusu
/// </summary>
public record GetCardApplicationByIdQuery(Guid Id) : IRequest<Result<CardApplicationDto>>;