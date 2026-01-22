using Card.Application.DTOs;
using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Queries;

public record GetCardApplicationsPagedQuery(CardApplicationFilterDto Filter) : IRequest<PagedResponse<CardApplicationDto>>;

public class GetCardApplicationsPagedQueryHandler : IRequestHandler<GetCardApplicationsPagedQuery, PagedResponse<CardApplicationDto>>
{
    private readonly ICardApplicationRepository _repository;

    public GetCardApplicationsPagedQueryHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResponse<CardApplicationDto>> Handle(GetCardApplicationsPagedQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;

        var (applications, totalCount) = await _repository.GetPagedAsync(
            filter.PageNumber,
            filter.PageSize,
            filter.StatusId,
            filter.CardTypeId,
            filter.CustomerTckn,
            filter.CustomerName,
            filter.StartDate,
            filter.EndDate,
            filter.SortBy,
            filter.SortDescending,
            cancellationToken);

        var dtos = applications.Select(CardApplicationDto.FromEntity).ToList();

        return PagedResponse<CardApplicationDto>.Create(dtos, totalCount, filter.PageNumber, filter.PageSize);
    }
}