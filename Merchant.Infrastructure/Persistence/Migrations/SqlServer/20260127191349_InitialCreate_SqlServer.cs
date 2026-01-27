using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Merchant.Infrastructure.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class InitialCreate_SqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Merchants",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TradeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaxNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TaxOffice = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MerchantTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ContractStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ContractEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Terminals",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TerminalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TerminalTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InstalledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstalledBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terminals_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalSchema: "dbo",
                        principalTable: "Merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_CreatedAt",
                schema: "dbo",
                table: "Merchants",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_Email",
                schema: "dbo",
                table: "Merchants",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_MerchantCode",
                schema: "dbo",
                table: "Merchants",
                column: "MerchantCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_TaxNumber",
                schema: "dbo",
                table: "Merchants",
                column: "TaxNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_CreatedAt",
                schema: "dbo",
                table: "Terminals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_MerchantId",
                schema: "dbo",
                table: "Terminals",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_StatusId",
                schema: "dbo",
                table: "Terminals",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_TerminalCode",
                schema: "dbo",
                table: "Terminals",
                column: "TerminalCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Terminals",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Merchants",
                schema: "dbo");
        }
    }
}
