using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transaction.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TransactionModuleIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BatchNumber_Status",
                table: "Transactions",
                columns: new[] { "BatchNumber", "StatusId" },
                filter: "[BatchNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Card_Status_Type_Date",
                table: "Transactions",
                columns: new[] { "CardNumberMasked", "StatusId", "TransactionTypeId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CardNumber_CreatedAt",
                table: "Transactions",
                columns: new[] { "CardNumberMasked", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Date_Status_Type",
                table: "Transactions",
                columns: new[] { "CreatedAt", "StatusId", "TransactionTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_MerchantId_CreatedAt",
                table: "Transactions",
                columns: new[] { "MerchantId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Status_CreatedAt",
                table: "Transactions",
                columns: new[] { "StatusId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TerminalId_CreatedAt",
                table: "Transactions",
                columns: new[] { "TerminalId", "CreatedAt" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_BatchNumber_Status",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Card_Status_Type_Date",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CardNumber_CreatedAt",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Date_Status_Type",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_MerchantId_CreatedAt",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Status_CreatedAt",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TerminalId_CreatedAt",
                table: "Transactions");
        }
    }
}
