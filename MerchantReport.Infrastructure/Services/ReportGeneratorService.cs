using MerchantReport.Domain.Entities;
using MerchantReport.Domain.Enums;
using MerchantReport.Domain.Repositories;
using MerchantReport.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ClosedXML.Excel;

namespace MerchantReport.Infrastructure.Services;

public class ReportGeneratorService : IReportGeneratorService
{
    private readonly IMerchantStatementRepository _statementRepository;
    private readonly string _outputPath;

    public ReportGeneratorService(IMerchantStatementRepository statementRepository)
    {
        _statementRepository = statementRepository;
        QuestPDF.Settings.License = LicenseType.Community;

        _outputPath = Path.Combine(Directory.GetCurrentDirectory(), "MerchantReports");
        if (!Directory.Exists(_outputPath))
            Directory.CreateDirectory(_outputPath);
    }

    public async Task<Result<ReportGenerationResult>> GenerateReportAsync(
        ReportRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Ekstre oluştur
            var statementResult = await GenerateMerchantStatementAsync(
                request.MerchantId,
                request.PeriodStart,
                request.PeriodEnd,
                cancellationToken);

            if (statementResult.IsFailure)
                return Result.Failure<ReportGenerationResult>(statementResult.Error!);

            var statement = statementResult.Value!;

            // Format'a göre dosya oluştur
            byte[] fileContent;
            string fileName;

            switch (request.ReportFormat.Name)
            {
                case nameof(ReportFormat.PDF):
                    fileContent = GeneratePdfReport(statement, request);
                    fileName = $"{request.RequestNumber}.pdf";
                    break;

                case nameof(ReportFormat.Excel):
                    fileContent = GenerateExcelReport(statement, request);
                    fileName = $"{request.RequestNumber}.xlsx";
                    break;

                case nameof(ReportFormat.CSV):
                    fileContent = GenerateCsvReport(statement, request);
                    fileName = $"{request.RequestNumber}.csv";
                    break;

                default:
                    return Result.Failure<ReportGenerationResult>("Desteklenmeyen rapor formatı");
            }

            // Dosyayı kaydet
            var filePath = Path.Combine(_outputPath, fileName);
            await File.WriteAllBytesAsync(filePath, fileContent, cancellationToken);

            return new ReportGenerationResult
            {
                FileName = fileName,
                FilePath = filePath,
                FileContent = fileContent,
                FileSize = fileContent.Length,
                TotalTransactions = statement.SalesCount + statement.RefundCount,
                TotalAmount = statement.TotalSales,
                TotalCommission = statement.TotalCommission,
                NetAmount = statement.TotalSales - statement.TotalCommission - statement.TotalRefunds
            };
        }
        catch (Exception ex)
        {
            return Result.Failure<ReportGenerationResult>($"Rapor oluşturma hatası: {ex.Message}");
        }
    }

    public async Task<Result<MerchantStatement>> GenerateMerchantStatementAsync(
        string merchantId,
        DateTime periodStart,
        DateTime periodEnd,
        CancellationToken cancellationToken = default)
    {
        // Önceki ekstrenin kapanış bakiyesini al
        var lastStatement = await _statementRepository.GetLatestByMerchantIdAsync(merchantId, cancellationToken);
        var openingBalance = lastStatement?.ClosingBalance ?? 0;

        var statementResult = MerchantStatement.Create(
            merchantId,
            $"Üye İşyeri {merchantId}",
            periodStart,
            periodEnd,
            openingBalance);

        if (statementResult.IsFailure)
            return Result.Failure<MerchantStatement>(statementResult.Error!);

        var statement = statementResult.Value!;

        // Demo veriler ekle (gerçek sistemde Transaction modülünden çekilir)
        AddDemoTransactions(statement, periodStart, periodEnd);

        await _statementRepository.AddAsync(statement, cancellationToken);
        await _statementRepository.SaveChangesAsync(cancellationToken);

        return statement;
    }

    private void AddDemoTransactions(MerchantStatement statement, DateTime periodStart, DateTime periodEnd)
    {
        var random = new Random();
        var days = (int)(periodEnd - periodStart).TotalDays;

        // Demo satış işlemleri
        for (int i = 0; i < random.Next(10, 30); i++)
        {
            var transactionDate = periodStart.AddDays(random.Next(0, days)).AddHours(random.Next(9, 21));
            var grossAmount = random.Next(50, 5000);
            var commissionRate = 1.5m + (decimal)random.NextDouble() * 1.5m;
            var commissionAmount = Math.Round(grossAmount * commissionRate / 100, 2);

            var item = MerchantStatementItem.Create(
                statement.Id,
                transactionDate,
                "Sale",
                grossAmount,
                commissionRate,
                commissionAmount,
                grossAmount - commissionAmount,
                Guid.NewGuid().ToString()[..8],
                $"REF{random.Next(100000, 999999)}",
                $"****{random.Next(1000, 9999)}",
                $"T{random.Next(100, 999)}",
                random.Next(1, 4),
                "POS Satışı");

            statement.AddItem(item);
        }

        // Demo iade işlemleri
        for (int i = 0; i < random.Next(1, 5); i++)
        {
            var transactionDate = periodStart.AddDays(random.Next(0, days)).AddHours(random.Next(9, 21));
            var grossAmount = random.Next(50, 500);
            var commissionRate = 1.5m;
            var commissionAmount = Math.Round(grossAmount * commissionRate / 100, 2);

            var item = MerchantStatementItem.Create(
                statement.Id,
                transactionDate,
                "Refund",
                -grossAmount,
                commissionRate,
                -commissionAmount,
                -(grossAmount - commissionAmount),
                Guid.NewGuid().ToString()[..8],
                $"REF{random.Next(100000, 999999)}",
                $"****{random.Next(1000, 9999)}",
                $"T{random.Next(100, 999)}",
                1,
                "İade İşlemi");

            statement.AddItem(item);
        }
    }

    private byte[] GeneratePdfReport(MerchantStatement statement, ReportRequest request)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(c => ComposePdfHeader(c, statement, request));
                page.Content().Element(c => ComposePdfContent(c, statement));
                page.Footer().Element(c => ComposePdfFooter(c));
            });
        });

        return document.GeneratePdf();
    }

    private void ComposePdfHeader(IContainer container, MerchantStatement statement, ReportRequest request)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("ÜYE İŞYERİ EKSTRESİ").Bold().FontSize(18);
                    col.Item().Text("Card Merchant Bank").FontSize(12);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Ekstre No: {statement.StatementNumber}").Bold();
                    col.Item().Text($"Tarih: {statement.StatementDate:dd.MM.yyyy}");
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Üye İşyeri: {statement.MerchantName}").Bold();
                    col.Item().Text($"Üye İşyeri No: {statement.MerchantId}");
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Dönem: {statement.PeriodStart:dd.MM.yyyy} - {statement.PeriodEnd:dd.MM.yyyy}");
                    col.Item().Text($"Rapor Tipi: {request.ReportType.DisplayName}");
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1);
        });
    }

    private void ComposePdfContent(IContainer container, MerchantStatement statement)
    {
        container.Column(column =>
        {
            // Özet
            column.Item().PaddingBottom(15).Border(1).Padding(10).Column(summary =>
            {
                summary.Item().Text("HESAP ÖZETİ").Bold().FontSize(12);
                summary.Item().PaddingTop(10);

                summary.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Açılış Bakiyesi:");
                        col.Item().Text("Toplam Satış:");
                        col.Item().Text("Toplam İade:");
                        col.Item().Text("Toplam Komisyon:");
                        col.Item().PaddingTop(5).Text("KAPANIŞ BAKİYESİ:").Bold();
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text($"{statement.OpeningBalance:N2} TL");
                        col.Item().Text($"{statement.TotalSales:N2} TL");
                        col.Item().Text($"{statement.TotalRefunds:N2} TL");
                        col.Item().Text($"{statement.TotalCommission:N2} TL");
                        col.Item().PaddingTop(5).Text($"{statement.ClosingBalance:N2} TL").Bold();
                    });
                });
            });

            // İşlem sayıları
            column.Item().PaddingBottom(15).Row(row =>
            {
                row.RelativeItem().Text($"Satış Adedi: {statement.SalesCount}");
                row.RelativeItem().Text($"İade Adedi: {statement.RefundCount}");
                row.RelativeItem().Text($"Chargeback Adedi: {statement.ChargebackCount}");
            });

            // İşlem detayları
            column.Item().Text("İŞLEM DETAYLARI").Bold().FontSize(12);
            column.Item().PaddingTop(10);

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(70);  // Tarih
                    columns.ConstantColumn(50);  // Tip
                    columns.ConstantColumn(60);  // Kart
                    columns.RelativeColumn();    // Referans
                    columns.ConstantColumn(70);  // Brüt
                    columns.ConstantColumn(50);  // Kom%
                    columns.ConstantColumn(60);  // Komisyon
                    columns.ConstantColumn(70);  // Net
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Tarih").Bold().FontSize(8);
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Tip").Bold().FontSize(8);
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Kart").Bold().FontSize(8);
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).Text("Referans").Bold().FontSize(8);
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Brüt").Bold().FontSize(8);
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Kom%").Bold().FontSize(8);
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Komisyon").Bold().FontSize(8);
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(3).AlignRight().Text("Net").Bold().FontSize(8);
                });

                foreach (var item in statement.Items.OrderBy(x => x.TransactionDate))
                {
                    var bgColor = item.TransactionType == "Refund" ? Colors.Red.Lighten5 : Colors.White;

                    table.Cell().Background(bgColor).BorderBottom(0.5f).Padding(3).Text(item.TransactionDate.ToString("dd.MM.yy HH:mm")).FontSize(8);
                    table.Cell().Background(bgColor).BorderBottom(0.5f).Padding(3).Text(item.TransactionType).FontSize(8);
                    table.Cell().Background(bgColor).BorderBottom(0.5f).Padding(3).Text(item.CardNumber ?? "-").FontSize(8);
                    table.Cell().Background(bgColor).BorderBottom(0.5f).Padding(3).Text(item.ReferenceNumber ?? "-").FontSize(8);
                    table.Cell().Background(bgColor).BorderBottom(0.5f).Padding(3).AlignRight().Text($"{item.GrossAmount:N2}").FontSize(8);
                    table.Cell().Background(bgColor).BorderBottom(0.5f).Padding(3).AlignRight().Text($"{item.CommissionRate:N2}%").FontSize(8);
                    table.Cell().Background(bgColor).BorderBottom(0.5f).Padding(3).AlignRight().Text($"{item.CommissionAmount:N2}").FontSize(8);
                    table.Cell().Background(bgColor).BorderBottom(0.5f).Padding(3).AlignRight().Text($"{item.NetAmount:N2}").FontSize(8);
                }
            });
        });
    }

    private void ComposePdfFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Card Merchant Bank - Üye İşyeri Hizmetleri: 0850 123 45 67").FontSize(8);
        });
    }

    private byte[] GenerateExcelReport(MerchantStatement statement, ReportRequest request)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ekstre");

        // Başlık
        worksheet.Cell("A1").Value = "ÜYE İŞYERİ EKSTRESİ";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;

        worksheet.Cell("A2").Value = $"Üye İşyeri: {statement.MerchantName}";
        worksheet.Cell("A3").Value = $"Dönem: {statement.PeriodStart:dd.MM.yyyy} - {statement.PeriodEnd:dd.MM.yyyy}";
        worksheet.Cell("A4").Value = $"Ekstre No: {statement.StatementNumber}";

        // Özet
        worksheet.Cell("A6").Value = "HESAP ÖZETİ";
        worksheet.Cell("A6").Style.Font.Bold = true;

        worksheet.Cell("A7").Value = "Açılış Bakiyesi:";
        worksheet.Cell("B7").Value = statement.OpeningBalance;
        worksheet.Cell("A8").Value = "Toplam Satış:";
        worksheet.Cell("B8").Value = statement.TotalSales;
        worksheet.Cell("A9").Value = "Toplam İade:";
        worksheet.Cell("B9").Value = statement.TotalRefunds;
        worksheet.Cell("A10").Value = "Toplam Komisyon:";
        worksheet.Cell("B10").Value = statement.TotalCommission;
        worksheet.Cell("A11").Value = "Kapanış Bakiyesi:";
        worksheet.Cell("B11").Value = statement.ClosingBalance;
        worksheet.Cell("A11").Style.Font.Bold = true;
        worksheet.Cell("B11").Style.Font.Bold = true;

        // İşlem detayları başlık
        var headerRow = 13;
        worksheet.Cell(headerRow, 1).Value = "Tarih";
        worksheet.Cell(headerRow, 2).Value = "Tip";
        worksheet.Cell(headerRow, 3).Value = "Kart No";
        worksheet.Cell(headerRow, 4).Value = "Referans";
        worksheet.Cell(headerRow, 5).Value = "Terminal";
        worksheet.Cell(headerRow, 6).Value = "Brüt Tutar";
        worksheet.Cell(headerRow, 7).Value = "Komisyon %";
        worksheet.Cell(headerRow, 8).Value = "Komisyon";
        worksheet.Cell(headerRow, 9).Value = "Net Tutar";
        worksheet.Cell(headerRow, 10).Value = "Taksit";

        worksheet.Range(headerRow, 1, headerRow, 10).Style.Font.Bold = true;
        worksheet.Range(headerRow, 1, headerRow, 10).Style.Fill.BackgroundColor = XLColor.LightGray;

        // İşlem detayları
        var row = headerRow + 1;
        foreach (var item in statement.Items.OrderBy(x => x.TransactionDate))
        {
            worksheet.Cell(row, 1).Value = item.TransactionDate;
            worksheet.Cell(row, 2).Value = item.TransactionType;
            worksheet.Cell(row, 3).Value = item.CardNumber;
            worksheet.Cell(row, 4).Value = item.ReferenceNumber;
            worksheet.Cell(row, 5).Value = item.TerminalId;
            worksheet.Cell(row, 6).Value = item.GrossAmount;
            worksheet.Cell(row, 7).Value = item.CommissionRate;
            worksheet.Cell(row, 8).Value = item.CommissionAmount;
            worksheet.Cell(row, 9).Value = item.NetAmount;
            worksheet.Cell(row, 10).Value = item.InstallmentCount;
            row++;
        }

        // Sütun genişlikleri
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private byte[] GenerateCsvReport(MerchantStatement statement, ReportRequest request)
    {
        var lines = new List<string>
        {
            "Tarih,Tip,Kart No,Referans,Terminal,Brüt Tutar,Komisyon %,Komisyon,Net Tutar,Taksit"
        };

        foreach (var item in statement.Items.OrderBy(x => x.TransactionDate))
        {
            lines.Add($"{item.TransactionDate:yyyy-MM-dd HH:mm},{item.TransactionType},{item.CardNumber},{item.ReferenceNumber},{item.TerminalId},{item.GrossAmount:F2},{item.CommissionRate:F2},{item.CommissionAmount:F2},{item.NetAmount:F2},{item.InstallmentCount}");
        }

        var content = string.Join(Environment.NewLine, lines);
        return System.Text.Encoding.UTF8.GetBytes(content);
    }
}