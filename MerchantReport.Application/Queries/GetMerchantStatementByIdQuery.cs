using CardMerchantSystem.Shared.Kernel;
using MediatR;
using MerchantReport.Application.DTOs;
using MerchantReport.Domain.Repositories;

namespace MerchantReport.Application.Queries;

public record GetMerchantStatementByIdQuery(Guid Id) : IRequest<Result<MerchantStatementDto>>;
public class GetMerchantStatementByIdQueryHandler
    : IRequestHandler<GetMerchantStatementByIdQuery, Result<MerchantStatementDto>>
{
    private readonly IMerchantStatementRepository _repository;

    public GetMerchantStatementByIdQueryHandler(IMerchantStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<MerchantStatementDto>> Handle(
        GetMerchantStatementByIdQuery request,
        CancellationToken cancellationToken)
    {
        var statement = await _repository.GetByIdWithItemsAsync(request.Id, cancellationToken);

        if (statement == null)
            return Result.Failure<MerchantStatementDto>("Ekstre bulunamadı", ErrorCodes.NotFound);

        return new MerchantStatementDto
        {
            Id = statement.Id,
            StatementNumber = statement.StatementNumber,
            MerchantId = statement.MerchantId,
            MerchantName = statement.MerchantName,
            PeriodStart = statement.PeriodStart,
            PeriodEnd = statement.PeriodEnd,
            StatementDate = statement.StatementDate,
            OpeningBalance = statement.OpeningBalance,
            TotalSales = statement.TotalSales,
            TotalRefunds = statement.TotalRefunds,
            TotalCommission = statement.TotalCommission,
            TotalSettlement = statement.TotalSettlement,
            ClosingBalance = statement.ClosingBalance,
            SalesCount = statement.SalesCount,
            RefundCount = statement.RefundCount,
            ChargebackCount = statement.ChargebackCount,
            CreatedAt = statement.CreatedAt,
            Items = statement.Items.Select(i => new MerchantStatementItemDto
            {
                Id = i.Id,
                TransactionDate = i.TransactionDate,
                TransactionType = i.TransactionType,
                TransactionId = i.TransactionId,
                ReferenceNumber = i.ReferenceNumber,
                CardNumber = i.CardNumber,
                TerminalId = i.TerminalId,
                GrossAmount = i.GrossAmount,
                CommissionRate = i.CommissionRate,
                CommissionAmount = i.CommissionAmount,
                NetAmount = i.NetAmount,
                InstallmentCount = i.InstallmentCount,
                Description = i.Description
            }).ToList()
        };
    }
}