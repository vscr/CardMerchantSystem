using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Repositories;
using MediatR;

namespace MerchantReport.Application.Queries;

public class GetMerchantStatementsByMerchantQueryHandler
    : IRequestHandler<GetMerchantStatementsByMerchantQuery, IReadOnlyList<MerchantStatementSummaryDto>>
{
    private readonly IMerchantStatementRepository _repository;

    public GetMerchantStatementsByMerchantQueryHandler(IMerchantStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MerchantStatementSummaryDto>> Handle(
        GetMerchantStatementsByMerchantQuery request,
        CancellationToken cancellationToken)
    {
        var statements = await _repository.GetByMerchantIdAsync(request.MerchantId, cancellationToken);

        return statements.Select(s => new MerchantStatementSummaryDto
        {
            Id = s.Id,
            StatementNumber = s.StatementNumber,
            MerchantName = s.MerchantName,
            PeriodStart = s.PeriodStart,
            PeriodEnd = s.PeriodEnd,
            TotalSales = s.TotalSales,
            TotalCommission = s.TotalCommission,
            ClosingBalance = s.ClosingBalance,
            SalesCount = s.SalesCount,
            CreatedAt = s.CreatedAt
        }).ToList();
    }
}