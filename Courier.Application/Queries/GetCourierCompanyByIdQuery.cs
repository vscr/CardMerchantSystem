using Courier.Application.DTOs;
using Courier.Domain.Entities;
using Courier.Domain.Repositories;
using MediatR;

namespace Courier.Application.Queries;

public record GetCourierCompanyByIdQuery(Guid Id) : IRequest<CourierCompanyDto?>;

public class GetCourierCompanyByIdQueryHandler : IRequestHandler<GetCourierCompanyByIdQuery, CourierCompanyDto?>
{
    private readonly ICourierCompanyRepository _repository;

    public GetCourierCompanyByIdQueryHandler(ICourierCompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<CourierCompanyDto?> Handle(GetCourierCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (company is null)
            return null;

        return MapToDto(company);
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