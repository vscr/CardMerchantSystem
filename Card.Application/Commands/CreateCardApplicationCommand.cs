using Card.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

/// <summary>
/// Yeni kart başvurusu oluşturma komutu
/// </summary>
public record CreateCardApplicationCommand(CreateCardApplicationDto Dto) : IRequest<Result<CardApplicationDto>>;