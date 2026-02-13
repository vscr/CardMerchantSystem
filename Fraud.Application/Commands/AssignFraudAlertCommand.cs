using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Commands;

public record AssignFraudAlertCommand(
    Guid FraudAlertId,
    string AssignTo
) : IRequest<bool>;

public class AssignFraudAlertCommandHandler : IRequestHandler<AssignFraudAlertCommand, bool>
{
    private readonly IFraudAlertRepository _repo;

    public AssignFraudAlertCommandHandler(IFraudAlertRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(AssignFraudAlertCommand cmd, CancellationToken ct)
    {
        var alert = await _repo.GetByIdAsync(cmd.FraudAlertId, ct);
        if (alert == null) return false;

        alert.AssignTo(cmd.AssignTo);
        await _repo.UpdateAsync(alert, ct);
        return true;
    }
}