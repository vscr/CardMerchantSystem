using MediatR;
using Microsoft.Extensions.Logging;
using Transaction.Application.DTOs;
using Transaction.Domain.Enums;
using Transaction.Domain.Repositories;

namespace Transaction.Application.Queries;

/// <summary>
/// İşlem istatistikleri query'si
/// </summary>
public record GetTransactionStatsQuery(DateTime StartDate, DateTime EndDate, Guid? MerchantId = null)
    : IRequest<TransactionStatsDto>;

public class GetTransactionStatsQueryHandler : IRequestHandler<GetTransactionStatsQuery, TransactionStatsDto>
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger<GetTransactionStatsQueryHandler> _logger;

    public GetTransactionStatsQueryHandler(
        ITransactionRepository repository,
        ILogger<GetTransactionStatsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TransactionStatsDto> Handle(
        GetTransactionStatsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Calculating transaction stats from {StartDate} to {EndDate}",
            request.StartDate,
            request.EndDate);

        // Tarih aralığındaki tüm işlemleri getir
        var transactions = await _repository.GetByDateRangeAsync(
            request.StartDate,
            request.EndDate,
            cancellationToken);

        // Merchant filtresi varsa uygula
        if (request.MerchantId.HasValue)
        {
            transactions = transactions
                .Where(t => t.MerchantId == request.MerchantId.Value)
                .ToList();
        }

        var stats = new TransactionStatsDto
        {
            PeriodStart = request.StartDate,
            PeriodEnd = request.EndDate,
            TotalCount = transactions.Count,
            TotalAmount = transactions.Sum(t => t.Amount.Amount)
        };

        // Başarılı işlemler (Approved + Settled)
        var successful = transactions
            .Where(t => t.Status.Id == TransactionStatus.Approved.Id ||
                        t.Status.Id == TransactionStatus.Settled.Id)
            .ToList();
        stats.SuccessfulCount = successful.Count;
        stats.SuccessfulAmount = successful.Sum(t => t.Amount.Amount);

        // Reddedilen işlemler
        var declined = transactions
            .Where(t => t.Status.Id == TransactionStatus.Declined.Id)
            .ToList();
        stats.DeclinedCount = declined.Count;
        stats.DeclinedAmount = declined.Sum(t => t.Amount.Amount);

        // İade işlemler (TransactionType = Refund olan başarılı işlemler)
        var refunded = transactions
            .Where(t => t.TransactionType.Id == TransactionType.Refund.Id &&
                        (t.Status.Id == TransactionStatus.Approved.Id ||
                         t.Status.Id == TransactionStatus.Settled.Id))
            .ToList();
        stats.RefundedCount = refunded.Count;
        stats.RefundedAmount = refunded.Sum(t => t.Amount.Amount);

        // Bekleyen işlemler
        stats.PendingCount = transactions
            .Count(t => t.Status.Id == TransactionStatus.Pending.Id);

        // İptal edilen işlemler (Reversed)
        var reversed = transactions
            .Where(t => t.Status.Id == TransactionStatus.Reversed.Id)
            .ToList();
        stats.ReversedCount = reversed.Count;
        stats.ReversedAmount = reversed.Sum(t => t.Amount.Amount);

        // Takas edilmiş işlemler
        var settled = transactions
            .Where(t => t.Status.Id == TransactionStatus.Settled.Id)
            .ToList();
        stats.SettledCount = settled.Count;
        stats.SettledAmount = settled.Sum(t => t.Amount.Amount);

        // İşlem tipi bazlı dağılım
        stats.ByTransactionType = transactions
            .GroupBy(t => new { t.TransactionType.Id, t.TransactionType.Name, t.TransactionType.DisplayName })
            .Select(g => new TransactionTypeStatsDto
            {
                TransactionType = g.Key.Name,
                TransactionTypeDisplayName = g.Key.DisplayName,
                Count = g.Count(),
                Amount = g.Sum(t => t.Amount.Amount),
                Percentage = stats.TotalCount > 0
                    ? Math.Round((decimal)g.Count() / stats.TotalCount * 100, 2)
                    : 0
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        // Günlük trend (son 7 gün veya istenilen dönem)
        var days = (request.EndDate - request.StartDate).Days;
        var trendDays = Math.Min(days, 7); // Maksimum 7 gün göster

        stats.DailyTrend = Enumerable.Range(0, trendDays + 1)
            .Select(i => request.EndDate.Date.AddDays(-trendDays + i))
            .Select(date =>
            {
                var dayTransactions = transactions
                    .Where(t => t.CreatedAt.Date == date)
                    .ToList();

                return new DailyTransactionStatsDto
                {
                    Date = date,
                    Count = dayTransactions.Count,
                    Amount = dayTransactions.Sum(t => t.Amount.Amount),
                    SuccessfulCount = dayTransactions.Count(t =>
                        t.Status.Id == TransactionStatus.Approved.Id ||
                        t.Status.Id == TransactionStatus.Settled.Id),
                    DeclinedCount = dayTransactions.Count(t =>
                        t.Status.Id == TransactionStatus.Declined.Id)
                };
            })
            .ToList();

        _logger.LogInformation(
            "Transaction stats calculated: Total={Total}, Successful={Successful}, Declined={Declined}",
            stats.TotalCount,
            stats.SuccessfulCount,
            stats.DeclinedCount);

        return stats;
    }
}