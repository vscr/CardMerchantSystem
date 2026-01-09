using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BKM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBKMModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BINTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BIN = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CardBrand = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CardType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CardLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BINTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClearingRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SwitchMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    STAN = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    RRN = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ClearingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AcquirerBankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IssuerBankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TerminalId = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    BIN = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClearingDate = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    IsSettled = table.Column<bool>(type: "bit", nullable: false),
                    SettledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SettlementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClearingRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SettlementBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettlementDate = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalTransactionCount = table.Column<int>(type: "int", nullable: false),
                    TotalTransactionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFeeAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalNetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettlementBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SwitchMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageTypeId = table.Column<int>(type: "int", nullable: false),
                    ProcessingCodeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    STAN = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    RRN = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CardNumberEncrypted = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiryDate = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TransactionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminalId = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MCC = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    AcquirerBankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IssuerBankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BIN = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ResponseCodeId = table.Column<int>(type: "int", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessingTimeMs = table.Column<int>(type: "int", nullable: false),
                    RoutingKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    OriginalMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwitchMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankSettlementSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SettlementBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsAcquirer = table.Column<bool>(type: "bit", nullable: false),
                    TransactionCount = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankSettlementSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankSettlementSummaries_SettlementBatches_SettlementBatchId",
                        column: x => x.SettlementBatchId,
                        principalTable: "SettlementBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankSettlementSummaries_BankCode",
                table: "BankSettlementSummaries",
                column: "BankCode");

            migrationBuilder.CreateIndex(
                name: "IX_BankSettlementSummaries_SettlementBatchId",
                table: "BankSettlementSummaries",
                column: "SettlementBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BINTables_BankCode",
                table: "BINTables",
                column: "BankCode");

            migrationBuilder.CreateIndex(
                name: "IX_BINTables_BIN",
                table: "BINTables",
                column: "BIN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BINTables_CardBrand",
                table: "BINTables",
                column: "CardBrand");

            migrationBuilder.CreateIndex(
                name: "IX_BINTables_IsActive",
                table: "BINTables",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_AcquirerBankCode",
                table: "ClearingRecords",
                column: "AcquirerBankCode");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_ClearingDate",
                table: "ClearingRecords",
                column: "ClearingDate");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_IsSettled",
                table: "ClearingRecords",
                column: "IsSettled");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_IssuerBankCode",
                table: "ClearingRecords",
                column: "IssuerBankCode");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_STAN",
                table: "ClearingRecords",
                column: "STAN");

            migrationBuilder.CreateIndex(
                name: "IX_ClearingRecords_SwitchMessageId",
                table: "ClearingRecords",
                column: "SwitchMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementBatches_BatchNumber",
                table: "SettlementBatches",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SettlementBatches_IsCompleted",
                table: "SettlementBatches",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_SettlementBatches_SettlementDate",
                table: "SettlementBatches",
                column: "SettlementDate");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_MerchantId",
                table: "SwitchMessages",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_ReceivedAt",
                table: "SwitchMessages",
                column: "ReceivedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_RRN",
                table: "SwitchMessages",
                column: "RRN");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_STAN",
                table: "SwitchMessages",
                column: "STAN");

            migrationBuilder.CreateIndex(
                name: "IX_SwitchMessages_TransactionDateTime",
                table: "SwitchMessages",
                column: "TransactionDateTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankSettlementSummaries");

            migrationBuilder.DropTable(
                name: "BINTables");

            migrationBuilder.DropTable(
                name: "ClearingRecords");

            migrationBuilder.DropTable(
                name: "SwitchMessages");

            migrationBuilder.DropTable(
                name: "SettlementBatches");
        }
    }
}
