using Fraud.Domain.Entities;
using Fraud.Domain.Repositories;
using MediatR;

namespace Fraud.Application.Commands;

public record AddToBlacklistCommand(
    string ListType,
    string Value,
    bool IsBlacklist,
    string? Reason,
    DateTime? ExpiresAt,
    string CreatedBy
) : IRequest<Guid>;

public class AddToBlacklistCommandHandler : IRequestHandler<AddToBlacklistCommand, Guid>
{
    private readonly IFraudBlacklistRepository _repo;

    public AddToBlacklistCommandHandler(IFraudBlacklistRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(AddToBlacklistCommand cmd, CancellationToken ct)
    {
        var exists = await _repo.ExistsAsync(cmd.ListType, cmd.Value, ct);
        if (exists)
            throw new InvalidOperationException($"Bu kayıt zaten mevcut: {cmd.ListType}/{cmd.Value}");

        var entry = new FraudBlacklist(
            cmd.ListType, cmd.Value, cmd.IsBlacklist,
            cmd.Reason, cmd.ExpiresAt, cmd.CreatedBy);

        await _repo.AddAsync(entry, ct);
        return entry.Id;
    }
}