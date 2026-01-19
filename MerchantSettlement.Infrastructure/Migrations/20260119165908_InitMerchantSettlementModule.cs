using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerchantSettlement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitMerchantSettlementModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MerchantDailySettlementSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettlementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalMerchantCount = table.Column<int>(type: "int", nullable: false),
                    TotalBatchCount = table.Column<int>(type: "int", nullable: false),
                    TotalTransactionCount = table.Column<int>(type: "int", nullable: false),
                    TotalSalesAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalRefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalChargebackAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalGrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalNetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SalesCount = table.Column<int>(type: "int", nullable: false),
                    RefundCount = table.Column<int>(type: "int", nullable: false),
                    ChargebackCount = table.Column<int>(type: "int", nullable: false),
                    CompletedBatchCount = table.Column<int>(type: "int", nullable: false),
                    FailedBatchCount = table.Column<int>(type: "int", nullable: false),
                    PendingBatchCount = table.Column<int>(type: "int", nullable: false),
                    IsFinalized = table.Column<bool>(type: "bit", nullable: false),
                    FinalizedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalizedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantDailySettlementSummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantPayouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayoutNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    WithholdingTax = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransferDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HoldReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SettlementBatchIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantPayouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantSettlementBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SettlementTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TotalSalesAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalRefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalChargebackAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    SalesCount = table.Column<int>(type: "int", nullable: false),
                    RefundCount = table.Column<int>(type: "int", nullable: false),
                    ChargebackCount = table.Column<int>(type: "int", nullable: false),
                    TotalTransactionCount = table.Column<int>(type: "int", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantSettlementBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantSettlementReconciliations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReconciliationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SettlementBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReconciliationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SystemGrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SystemCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SystemNetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SystemTransactionCount = table.Column<int>(type: "int", nullable: false),
                    ReportedGrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportedCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportedNetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReportedTransactionCount = table.Column<int>(type: "int", nullable: false),
                    GrossAmountDifference = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionDifference = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmountDifference = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionCountDifference = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResolvedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantSettlementReconciliations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantSettlementDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettlementBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CardBrand = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TerminalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(5,4)", precision: 5, scale: 4, nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    OriginalTransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantSettlementDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantSettlementDetails_MerchantSettlementBatches_SettlementBatchId",
                        column: x => x.SettlementBatchId,
                        principalTable: "MerchantSettlementBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantReconciliationMismatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReconciliationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MismatchType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SystemAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ReportedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    AmountDifference = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantReconciliationMismatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantReconciliationMismatches_MerchantSettlementReconciliations_ReconciliationId",
                        column: x => x.ReconciliationId,
                        principalTable: "MerchantSettlementReconciliations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerchantDailySettlementSummaries_SettlementDate",
                table: "MerchantDailySettlementSummaries",
                column: "SettlementDate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MerchantPayouts_MerchantId",
                table: "MerchantPayouts",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantPayouts_PayoutNumber",
                table: "MerchantPayouts",
                column: "PayoutNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MerchantReconciliationMismatches_ReconciliationId",
                table: "MerchantReconciliationMismatches",
                column: "ReconciliationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSettlementBatches_BatchNumber",
                table: "MerchantSettlementBatches",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSettlementBatches_MerchantId",
                table: "MerchantSettlementBatches",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSettlementDetails_SettlementBatchId",
                table: "MerchantSettlementDetails",
                column: "SettlementBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSettlementDetails_TransactionId",
                table: "MerchantSettlementDetails",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSettlementReconciliations_MerchantId",
                table: "MerchantSettlementReconciliations",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSettlementReconciliations_ReconciliationNumber",
                table: "MerchantSettlementReconciliations",
                column: "ReconciliationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MerchantSettlementReconciliations_SettlementBatchId",
                table: "MerchantSettlementReconciliations",
                column: "SettlementBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantDailySettlementSummaries");

            migrationBuilder.DropTable(
                name: "MerchantPayouts");

            migrationBuilder.DropTable(
                name: "MerchantReconciliationMismatches");

            migrationBuilder.DropTable(
                name: "MerchantSettlementDetails");

            migrationBuilder.DropTable(
                name: "MerchantSettlementReconciliations");

            migrationBuilder.DropTable(
                name: "MerchantSettlementBatches");
        }
    }
}
