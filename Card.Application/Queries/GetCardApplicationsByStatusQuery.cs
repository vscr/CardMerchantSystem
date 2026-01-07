using Card.Application.DTOs;
using MediatR;

namespace Card.Application.Queries;

/// <summary>
/// Duruma göre başvuruları getirme sorgusu
/// </summary>
public record GetCardApplicationsByStatusQuery(int StatusId) : IRequest<IReadOnlyList<CardApplicationDto>>;