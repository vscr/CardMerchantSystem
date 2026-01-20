using Courier.Application.DTOs;
using Courier.Domain.Entities;
using Courier.Domain.Repositories;
using MediatR;

namespace Courier.Application.Queries;

public record GetAllCourierCompaniesQuery(bool ActiveOnly = false) : IRequest<IReadOnlyList<CourierCompanyDto>>;

public class GetAllCourierCompaniesQueryHandler : IRequestHandler<GetAllCourierCompaniesQuery, IReadOnlyList<CourierCompanyDto>>
{
    private readonly ICourierCompanyRepository _repository;

    public GetAllCourierCompaniesQueryHandler(ICourierCompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<CourierCompanyDto>> Handle(GetAllCourierCompaniesQuery request, CancellationToken cancellationToken)
    {
        var companies = request.ActiveOnly
            ? await _repository.GetActiveAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return companies.Select(MapToDto).ToList();
    }

    private static CourierCompanyDto MapToDto(CourierCompany company)
    {
        return new CourierCompanyDto
        {
            Id = company.Id,
            Code = company.Code,
            Name = company.Name,
            CompanyType = company.CompanyType.Name,
            CompanyTypeDisplayName = company.CompanyType.DisplayName,
            ContactPerson = company.ContactPerson,
            ContactPhone = company.ContactPhone,
            ContactEmail = company.ContactEmail,
            ApiEndpoint = company.ApiEndpoint,
            TrackingUrlTemplate = company.TrackingUrlTemplate,
            IsActive = company.IsActive,
            StandardDeliveryDays = company.StandardDeliveryDays,
            ExpressDeliveryDays = company.ExpressDeliveryDays,
            BasePrice = company.BasePrice,
            PricePerKg = company.PricePerKg,
            CreatedAt = company.CreatedAt
        };
    }
}