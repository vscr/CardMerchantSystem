using BKM.Application.DTOs;
using BKM.Domain.Repositories;
using MediatR;

namespace BKM.Application.Queries;

public class GetAllBINsQueryHandler
    : IRequestHandler<GetAllBINsQuery, IReadOnlyList<BINTableDto>>
{
    private readonly IBINTableRepository _repository;

    public GetAllBINsQueryHandler(IBINTableRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BINTableDto>> Handle(
        GetAllBINsQuery request,
        CancellationToken cancellationToken)
    {
        var bins = await _repository.GetAllActiveAsync(cancellationToken);

        return bins.Select(b => new BINTableDto
        {
            Id = b.Id,
            BIN = b.BIN,
            BankCode = b.BankCode,
            BankName = b.BankName,
            CardBrand = b.CardBrand,
            CardType = b.CardType,
            CardLevel = b.CardLevel,
            IsActive = b.IsActive
        }).ToList();
    }
}