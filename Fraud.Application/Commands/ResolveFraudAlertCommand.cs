using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using Fraud.Domain.Events;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Commands;

public record ResolveFraudAlertCommand(
    Guid FraudAlertId,
    FraudDecision Decision,
    string? Comment,
    string? CardStatusAction,
    string? CardStatusReasonCode,
    string ActionBy
) : IRequest<Guid>;

public class ResolveFraudAlertCommandHandler : IRequestHandler<ResolveFraudAlertCommand, Guid>
{
    private readonly IFraudAlertRepository _alertRepo;
    private readonly IFraudActionRepository _actionRepo;
    private readonly ICardFraudProfileRepository _profileRepo;
    private readonly IMediator _mediator;

    public ResolveFraudAlertCommandHandler(
        IFraudAlertRepository alertRepo,
        IFraudActionRepository actionRepo,
        ICardFraudProfileRepository profileRepo,
        IMediator mediator)
    {
        _alertRepo = alertRepo;
        _actionRepo = actionRepo;
        _profileRepo = profileRepo;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(ResolveFraudAlertCommand cmd, CancellationToken ct)
    {
        var alert = await _alertRepo.GetByIdAsync(cmd.FraudAlertId, ct)
            ?? throw new KeyNotFoundException($"Alert bulunamadı: {cmd.FraudAlertId}");

        // 1. Aksiyon kaydı
        var action = new FraudAction(
            cmd.FraudAlertId, alert.TransactionId, alert.MaskedCardNo,
            cmd.Decision, cmd.Comment,
            cmd.CardStatusAction, cmd.CardStatusReasonCode,
            cmd.ActionBy);

        await _actionRepo.AddAsync(action, ct);

        // 2. Alert çözümle
        alert.Resolve(cmd.Decision, cmd.Comment ?? string.Empty);
        await _alertRepo.UpdateAsync(alert, ct);

        // 3. Fraud onaylandıysa profil güncelle
        if (cmd.Decision == FraudDecision.ConfirmedFraud)
        {
            var profile = await _profileRepo.GetOrCreateAsync(alert.MaskedCardNo, ct);
            profile.RecordConfirmedFraud();
            await _profileRepo.UpdateAsync(profile, ct);
        }

        // 4. Kart bloke event
        if (cmd.CardStatusAction == "Blocked")
        {
            await _mediator.Publish(new CardBlockedByFraudEvent(
                cmd.FraudAlertId, alert.MaskedCardNo,
                cmd.CardStatusReasonCode ?? "FRAUD",
                cmd.ActionBy), ct);
        }

        // 5. Alert resolved event
        await _mediator.Publish(new FraudAlertResolvedEvent(
            cmd.FraudAlertId, alert.TransactionId, alert.MaskedCardNo,
            cmd.Decision, cmd.CardStatusAction, cmd.ActionBy), ct);

        return action.Id;
    }
}