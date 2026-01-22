using Card.Application.DTOs;
using Card.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace Card.Application.Commands;

public record StartReviewCommand(Guid ApplicationId, string ReviewerUsername) : IRequest<Result<CardApplicationDto>>;

public class StartReviewCommandHandler : IRequestHandler<StartReviewCommand, Result<CardApplicationDto>>
{
    private readonly ICardApplicationRepository _repository;

    public StartReviewCommandHandler(ICardApplicationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CardApplicationDto>> Handle(StartReviewCommand request, CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);
        if (application is null)
            return Result.Failure<CardApplicationDto>("Başvuru bulunamadı", ErrorCodes.CardApplicationNotFound);

        var result = application.StartReview(request.ReviewerUsername);
        if (result.IsFailure)
            return Result.Failure<CardApplicationDto>(result.Error, result.ErrorCode);

        await _repository.UpdateAsync(application, cancellationToken);

        return Result.Success(CardApplicationDto.FromEntity(application));
    }
}