using BKM.Application.DTOs;
using BKM.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BKM.Application.Queries;

public record GetBINInfoQuery(string BIN) : IRequest<Result<BINTableDto>>;
public class GetBINInfoQueryHandler
    : IRequestHandler<GetBINInfoQuery, Result<BINTableDto>>
{
    private readonly IBINTableRepository _repository;

    public GetBINInfoQueryHandler(IBINTableRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BINTableDto>> Handle(
        GetBINInfoQuery request,
        CancellationToken cancellationToken)
    {
        var binTable = await _repository.GetByBINAsync(request.BIN, cancellationToken);

        if (binTable == null)
            return Result.Failure<BINTableDto>("BIN bulunamadı", ErrorCodes.NotFound);

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