using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerchantReport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMerchantReportModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MerchantReportConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: false),
                    ReportFormatId = table.Column<int>(type: "int", nullable: false),
                    DeliveryMethodId = table.Column<int>(type: "int", nullable: false),
                    FrequencyId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    DayOfMonth = table.Column<int>(type: "int", nullable: false),
                    RunTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    NextRunTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRunTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailRecipients = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FtpHost = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FtpPort = table.Column<int>(type: "int", nullable: true),
                    FtpUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FtpPassword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FtpPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UseSftp = table.Column<bool>(type: "bit", nullable: false),
                    CallbackUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CallbackApiKey = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantReportConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantStatements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpeningBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalSales = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalRefunds = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalSettlement = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClosingBalance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SalesCount = table.Column<int>(type: "int", nullable: false),
                    RefundCount = table.Column<int>(type: "int", nullable: false),
                    ChargebackCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantStatements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReportTypeId = table.Column<int>(type: "int", nullable: false),
                    ReportFormatId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    DeliveryMethodId = table.Column<int>(type: "int", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDetails = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    TotalTransactions = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RequestedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReportConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantStatementItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CardNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TerminalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantStatementItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantStatementItems_MerchantStatements_StatementId",
                        column: x => x.StatementId,
                        principalTable: "MerchantStatements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerchantReportConfigs_IsActive",
                table: "MerchantReportConfigs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantReportConfigs_MerchantId",
                table: "MerchantReportConfigs",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantReportConfigs_NextRunTime",
                table: "MerchantReportConfigs",
                column: "NextRunTime");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStatementItems_StatementId",
                table: "MerchantStatementItems",
                column: "StatementId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStatementItems_TransactionDate",
                table: "MerchantStatementItems",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStatementItems_TransactionType",
                table: "MerchantStatementItems",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStatements_MerchantId",
                table: "MerchantStatements",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStatements_PeriodEnd",
                table: "MerchantStatements",
                column: "PeriodEnd");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStatements_PeriodStart",
                table: "MerchantStatements",
                column: "PeriodStart");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStatements_StatementNumber",
                table: "MerchantStatements",
                column: "StatementNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_CreatedAt",
                table: "ReportRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_MerchantId",
                table: "ReportRequests",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_PeriodEnd",
                table: "ReportRequests",
                column: "PeriodEnd");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_PeriodStart",
                table: "ReportRequests",
                column: "PeriodStart");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_RequestNumber",
                table: "ReportRequests",
                column: "RequestNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_StatusId",
                table: "ReportRequests",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantReportConfigs");

            migrationBuilder.DropTable(
                name: "MerchantStatementItems");

            migrationBuilder.DropTable(
                name: "ReportRequests");

            migrationBuilder.DropTable(
                name: "MerchantStatements");
        }
    }
}
