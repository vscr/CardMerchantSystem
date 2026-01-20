using CardMerchantSystem.Shared.Kernel;
using Courier.Application.DTOs;
using Courier.Domain.Entities;
using Courier.Domain.Enums;
using Courier.Domain.Repositories;
using MediatR;

namespace Courier.Application.Commands;

public record CreateCourierCompanyCommand(CreateCourierCompanyDto Dto) : IRequest<Result<CourierCompanyDto>>;

public class CreateCourierCompanyCommandHandler : IRequestHandler<CreateCourierCompanyCommand, Result<CourierCompanyDto>>
{
    private readonly ICourierCompanyRepository _repository;

    public CreateCourierCompanyCommandHandler(ICourierCompanyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CourierCompanyDto>> Handle(CreateCourierCompanyCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var existing = await _repository.GetByCodeAsync(dto.Code, cancellationToken);
        if (existing is not null)
            return Result.Failure<CourierCompanyDto>("Bu kodla firma zaten mevcut");

        var companyType = Enumeration.FromId<CourierCompanyType>(dto.CompanyTypeId);
        if (companyType is null)
            return Result.Failure<CourierCompanyDto>("Geçersiz firma tipi");

        var companyResult = CourierCompany.Create(
            dto.Code,
            dto.Name,
            companyType,
            dto.ContactPerson,
            dto.ContactPhone,
            dto.ContactEmail,
            dto.StandardDeliveryDays,
            dto.BasePrice);

        if (companyResult.IsFailure)
            return Result.Failure<CourierCompanyDto>(companyResult.Error);

        var company = companyResult.Value!;

        if (!string.IsNullOrWhiteSpace(dto.ApiEndpoint))
            company.SetApiCredentials(dto.ApiEndpoint, dto.ApiKey ?? "");

        if (!string.IsNullOrWhiteSpace(dto.TrackingUrlTemplate))
            company.SetTrackingUrlTemplate(dto.TrackingUrlTemplate);

        await _repository.AddAsync(company, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

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