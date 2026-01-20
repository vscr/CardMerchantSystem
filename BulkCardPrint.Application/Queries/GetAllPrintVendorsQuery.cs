using MediatR;
using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Repositories;

namespace BulkCardPrint.Application.Queries;

public record GetAllPrintVendorsQuery(bool ActiveOnly = false) : IRequest<IReadOnlyList<PrintVendorDto>>;

public class GetAllPrintVendorsQueryHandler : IRequestHandler<GetAllPrintVendorsQuery, IReadOnlyList<PrintVendorDto>>
{
    private readonly IPrintVendorRepository _repository;

    public GetAllPrintVendorsQueryHandler(IPrintVendorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PrintVendorDto>> Handle(GetAllPrintVendorsQuery request, CancellationToken cancellationToken)
    {
        var vendors = request.ActiveOnly
            ? await _repository.GetActiveAsync(cancellationToken)
            : await _repository.GetAllAsync(cancellationToken);

        return vendors.Select(MapToDto).ToList();
    }

    private static PrintVendorDto MapToDto(PrintVendor vendor)
    {
        return new PrintVendorDto
        {
            Id = vendor.Id,
            Code = vendor.Code,
            Name = vendor.Name,
            ContactPerson = vendor.ContactPerson,
            ContactEmail = vendor.ContactEmail,
            ContactPhone = vendor.ContactPhone,
            ApiEndpoint = vendor.ApiEndpoint,
            FtpHost = vendor.FtpHost,
            PreferredFileFormat = vendor.PreferredFileFormat.Name,
            PreferredFileFormatDisplayName = vendor.PreferredFileFormat.DisplayName,
            IsActive = vendor.IsActive,
            DailyCapacity = vendor.DailyCapacity,
            CurrentDailyLoad = vendor.CurrentDailyLoad,
            RemainingCapacity = vendor.RemainingCapacity,
            CreatedAt = vendor.CreatedAt
        };
    }
}