using BulkCardPrint.Application.DTOs;
using BulkCardPrint.Domain.Entities;
using BulkCardPrint.Domain.Enums;
using BulkCardPrint.Domain.Repositories;
using CardMerchantSystem.Shared.Kernel;
using MediatR;

namespace BulkCardPrint.Application.Commands;

public record CreatePrintVendorCommand(CreatePrintVendorDto Dto) : IRequest<Result<PrintVendorDto>>;

public class CreatePrintVendorCommandHandler : IRequestHandler<CreatePrintVendorCommand, Result<PrintVendorDto>>
{
    private readonly IPrintVendorRepository _repository;

    public CreatePrintVendorCommandHandler(IPrintVendorRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PrintVendorDto>> Handle(CreatePrintVendorCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        // Kod kontrolü
        var existingVendor = await _repository.GetByCodeAsync(dto.Code, cancellationToken);
        if (existingVendor is not null)
            return Result.Failure<PrintVendorDto>("Bu kodla firma zaten mevcut");

        var fileFormat = Enumeration.FromId<FileFormat>(dto.FileFormatId);
        if (fileFormat is null)
            return Result.Failure<PrintVendorDto>("Geçersiz dosya formatı");

        var vendorResult = PrintVendor.Create(
            dto.Code,
            dto.Name,
            dto.ContactPerson,
            dto.ContactEmail,
            dto.ContactPhone,
            fileFormat,
            dto.DailyCapacity);

        if (vendorResult.IsFailure)
            return Result.Failure<PrintVendorDto>(vendorResult.Error);

        var vendor = vendorResult.Value!;

        if (!string.IsNullOrWhiteSpace(dto.ApiEndpoint))
            vendor.SetApiEndpoint(dto.ApiEndpoint);

        if (!string.IsNullOrWhiteSpace(dto.FtpHost))
            vendor.SetFtpCredentials(dto.FtpHost, dto.FtpUsername ?? "", dto.FtpPath ?? "");

        await _repository.AddAsync(vendor, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

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