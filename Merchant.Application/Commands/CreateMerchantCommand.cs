using Merchant.Application.DTOs;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Merchant.Application.Commands;

/// <summary>
/// Yeni üye işyeri oluşturma komutu
/// </summary>
public record CreateMerchantCommand(CreateMerchantDto Dto) : IRequest<Result<MerchantDto>>;