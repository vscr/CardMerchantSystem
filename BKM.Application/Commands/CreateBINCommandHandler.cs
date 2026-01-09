using BKM.Application.DTOs;
using BKM.Domain.Entities;
using BKM.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Commands;

public class CreateBINCommandHandler
    : IRequestHandler<CreateBINCommand, Result<BINTableDto>>
{
    private readonly IBINTableRepository _repository;

    public CreateBINCommandHandler(IBINTableRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BINTableDto>> Handle(
        CreateBINCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // BIN zaten var mı kontrol et
        var existing = await _repository.GetByBINAsync(dto.BIN, cancellationToken);
        if (existing != null)
            return Result.Failure<BINTableDto>("Bu BIN zaten kayıtlı");

        // BIN oluştur
        var binTable = BINTable.Create(
            dto.BIN,
            dto.BankCode,
            dto.BankName,
            dto.CardBrand,
            dto.CardType,
            dto.CardLevel);

        await _repository.AddAsync(binTable, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new BINTableDto
        {
            Id = binTable.Id,
            BIN = binTable.BIN,
            BankCode = binTable.BankCode,
            BankName = binTable.BankName,
            CardBrand = binTable.CardBrand,
            CardType = binTable.CardType,
            CardLevel = binTable.CardLevel,
            IsActive = binTable.IsActive
        };
    }
}