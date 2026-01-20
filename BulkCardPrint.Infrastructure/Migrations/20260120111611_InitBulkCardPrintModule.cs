using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkCardPrint.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitBulkCardPrintModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrintVendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ApiEndpoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FtpHost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FtpUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FtpPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileFormatId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DailyCapacity = table.Column<int>(type: "int", nullable: false),
                    CurrentDailyLoad = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintVendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrintBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PrintVendorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TotalItemCount = table.Column<int>(type: "int", nullable: false),
                    PrintedCount = table.Column<int>(type: "int", nullable: false),
                    FailedCount = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileGeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FileChecksum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SentToVendorAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductionStartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintBatches_PrintVendors_PrintVendorId",
                        column: x => x.PrintVendorId,
                        principalTable: "PrintVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PrintBatchItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrintBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerSurname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerTckn = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CardType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CardNumberEncrypted = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ExpiryDate = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    Cvv = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PrintedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QualityCheckedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SequenceNumber = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintBatchItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintBatchItems_PrintBatches_PrintBatchId",
                        column: x => x.PrintBatchId,
                        principalTable: "PrintBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrintBatches_BatchNumber",
                table: "PrintBatches",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrintBatches_PrintVendorId",
                table: "PrintBatches",
                column: "PrintVendorId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintBatchItems_CardApplicationId",
                table: "PrintBatchItems",
                column: "CardApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintBatchItems_PrintBatchId",
                table: "PrintBatchItems",
                column: "PrintBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintVendors_Code",
                table: "PrintVendors",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintBatchItems");

            migrationBuilder.DropTable(
                name: "PrintBatches");

            migrationBuilder.DropTable(
                name: "PrintVendors");
        }
    }
}
