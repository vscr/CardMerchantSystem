using Statement.Domain.Entities;
using Statement.Domain.Services;
using CardMerchantSystem.Shared.Kernel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Statement.Infrastructure.Services;

public class StatementPdfService : IStatementPdfService
{
    private readonly string _outputPath;

    public StatementPdfService()
    {
        // QuestPDF lisans ayarı (Community Edition)
        QuestPDF.Settings.License = LicenseType.Community;

        _outputPath = Path.Combine(Directory.GetCurrentDirectory(), "StatementPdfs");
        if (!Directory.Exists(_outputPath))
            Directory.CreateDirectory(_outputPath);
    }

    public Task<Result<byte[]>> GeneratePdfAsync(
        CardStatement statement,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Header
                    page.Header().Element(c => ComposeHeader(c, statement));

                    // Content
                    page.Content().Element(c => ComposeContent(c, statement));

                    // Footer
                    page.Footer().Element(c => ComposeFooter(c, statement));
                });
            });

            var pdfBytes = document.GeneratePdf();
            return Task.FromResult(Result.Success(pdfBytes));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Failure<byte[]>($"PDF oluşturma hatası: {ex.Message}"));
        }
    }

    public Task<Result<string>> SavePdfAsync(
        CardStatement statement,
        byte[] pdfContent,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var fileName = $"Statement_{statement.StatementNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(_outputPath, fileName);

            File.WriteAllBytes(filePath, pdfContent);

            return Task.FromResult(Result.Success(filePath));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Failure<string>($"PDF kaydetme hatası: {ex.Message}"));
        }
    }

    private void ComposeHeader(IContainer container, CardStatement statement)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("KART EKSTRESİ").Bold().FontSize(18);
                    col.Item().Text("Card Merchant Bank").FontSize(12);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Ekstre No: {statement.StatementNumber}").Bold();
                    col.Item().Text($"Ekstre Tarihi: {statement.StatementDate:dd.MM.yyyy}");
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Sayın {statement.CustomerName}").Bold();
                    col.Item().Text($"Kart No: {statement.MaskedCardNumber}");
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"Dönem: {statement.PeriodStartDate:dd.MM.yyyy} - {statement.PeriodEndDate:dd.MM.yyyy}");
                    col.Item().Text($"Son Ödeme Tarihi: {statement.DueDate:dd.MM.yyyy}").Bold().FontColor(Colors.Red.Medium);
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1);
        });
    }

    private void ComposeContent(IContainer container, CardStatement statement)
    {
        container.Column(column =>
        {
            // Özet Bilgiler
            column.Item().PaddingBottom(15).Element(c => ComposeSummary(c, statement));

            // İşlem Detayları
            column.Item().Element(c => ComposeTransactions(c, statement));
        });
    }

    private void ComposeSummary(IContainer container, CardStatement statement)
    {
        container.Border(1).Padding(10).Column(column =>
        {
            column.Item().Text("HESAP ÖZETİ").Bold().FontSize(12);
            column.Item().PaddingTop(10);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Önceki Dönem Bakiyesi:");
                    col.Item().Text("Dönem İçi Harcamalar:");
                    col.Item().Text("Dönem İçi Ödemeler:");
                    col.Item().Text("Faiz Tutarı:");
                    col.Item().PaddingTop(5).Text("DÖNEM SONU BAKİYE:").Bold();
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"{statement.PreviousBalance:N2} TL");
                    col.Item().Text($"{statement.TotalDebits:N2} TL");
                    col.Item().Text($"{statement.TotalCredits:N2} TL");
                    col.Item().Text($"{statement.InterestAmount:N2} TL");
                    col.Item().PaddingTop(5).Text($"{statement.CurrentBalance:N2} TL").Bold();
                });
            });

            column.Item().PaddingTop(10).LineHorizontal(0.5f);

            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Minimum Ödeme Tutarı:").Bold();
                    col.Item().Text("Kullanılabilir Limit:");
                    col.Item().Text("Toplam Limit:");
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().Text($"{statement.MinimumPayment:N2} TL").Bold().FontColor(Colors.Red.Medium);
                    col.Item().Text($"{statement.AvailableCredit:N2} TL");
                    col.Item().Text($"{statement.CreditLimit:N2} TL");
                });
            });
        });
    }

    private void ComposeTransactions(IContainer container, CardStatement statement)
    {
        container.Column(column =>
        {
            column.Item().PaddingBottom(10).Text("İŞLEM DETAYLARI").Bold().FontSize(12);

            column.Item().Table(table =>
            {
                // Tablo başlıkları
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(70);  // Tarih
                    columns.RelativeColumn(3);    // Açıklama
                    columns.RelativeColumn(1.5f); // İşlem Tipi
                    columns.ConstantColumn(80);   // Tutar
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Tarih").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Açıklama").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("İşlem Tipi").Bold();
                    header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("Tutar").Bold();
                });

                // Tablo satırları
                foreach (var item in statement.Items.OrderBy(x => x.TransactionDate))
                {
                    var backgroundColor = item.ItemType.IsCredit ? Colors.Green.Lighten5 : Colors.White;

                    table.Cell().Background(backgroundColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1)
                        .Padding(5).Text(item.TransactionDate.ToString("dd.MM.yyyy"));

                    table.Cell().Background(backgroundColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1)
                        .Padding(5).Text(item.Description);

                    table.Cell().Background(backgroundColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1)
                        .Padding(5).Text(item.ItemType.DisplayName);

                    var amountColor = item.ItemType.IsCredit ? Colors.Green.Medium : Colors.Black;
                    table.Cell().Background(backgroundColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1)
                        .Padding(5).AlignRight().Text($"{item.Amount:N2} TL").FontColor(amountColor);
                }
            });
        });
    }

    private void ComposeFooter(IContainer container, CardStatement statement)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(10).LineHorizontal(1);

            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Önemli: ").Bold();
                    text.Span($"Son ödeme tarihi olan {statement.DueDate:dd.MM.yyyy} tarihine kadar en az minimum ödeme tutarı olan ");
                    text.Span($"{statement.MinimumPayment:N2} TL").Bold();
                    text.Span(" ödenmelidir. Aksi halde gecikme faizi uygulanacaktır.");
                });
            });

            column.Item().PaddingTop(10).AlignCenter().Text(text =>
            {
                text.Span("Card Merchant Bank - Müşteri Hizmetleri: 0850 123 45 67").FontSize(8);
            });
        });
    }
}