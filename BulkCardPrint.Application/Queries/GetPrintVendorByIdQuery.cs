using MediatR;
using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Repositories;

namespace BulkCardPrint.Application.Queries;

public record GetPrintVendorByIdQuery(Guid Id) : IRequest<PrintVendorDto?>;

public class GetPrintVendorByIdQueryHandler : IRequestHandler<GetPrintVendorByIdQuery, PrintVendorDto?>
{
    private readonly IPrintVendorRepository _repository;

    public GetPrintVendorByIdQueryHandler(IPrintVendorRepository repository)
    {
        _repository = repository;
    }

    public async Task<PrintVendorDto?> Handle(GetPrintVendorByIdQuery request, CancellationToken cancellationToken)
    {
        var vendor = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (vendor is null)
            return null;

        return MapToDto(vendor);
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