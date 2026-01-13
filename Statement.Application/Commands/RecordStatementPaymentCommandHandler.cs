using Statement.Application.DTOs;
using Statement.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Statement.Application.Commands;

public class RecordStatementPaymentCommandHandler
    : IRequestHandler<RecordStatementPaymentCommand, Result<StatementPaymentResultDto>>
{
    private readonly ICardStatementRepository _repository;

    public RecordStatementPaymentCommandHandler(ICardStatementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<StatementPaymentResultDto>> Handle(
        RecordStatementPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var statement = await _repository.GetByIdAsync(dto.StatementId, cancellationToken);
        if (statement == null)
            return Result.Failure<StatementPaymentResultDto>("Ekstre bulunamadı");

        var paymentResult = statement.RecordPayment(dto.Amount);
        if (paymentResult.IsFailure)
            return Result.Failure<StatementPaymentResultDto>(paymentResult.Error!);

        await _repository.UpdateAsync(statement, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new StatementPaymentResultDto
        {
            StatementId = statement.Id,
            StatementNumber = statement.StatementNumber,
            PaymentAmount = dto.Amount,
            TotalPaidAmount = statement.PaidAmount,
            RemainingBalance = statement.RemainingBalance,
            Status = statement.Status.Name,
            StatusDisplayName = statement.Status.DisplayName,
            PaymentDate = DateTime.UtcNow
        };
    }
}