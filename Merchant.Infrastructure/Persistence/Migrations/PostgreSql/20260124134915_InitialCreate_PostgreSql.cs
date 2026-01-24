using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Merchant.Infrastructure.Persistence.Migrations.PostgreSql
{
    /// <inheritdoc />
    public partial class InitialCreate_PostgreSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Merchants",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TradeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TaxNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TaxOffice = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MerchantTypeId = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    District = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IBAN = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ContractStartDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ContractEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Terminals",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MerchantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TerminalTypeId = table.Column<int>(type: "integer", nullable: false),
                    StatusId = table.Column<int>(type: "integer", nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    InstalledAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InstalledBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terminals_Merchants_MerchantId",
                        column: x => x.MerchantId,
                        principalSchema: "public",
                        principalTable: "Merchants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_CreatedAt",
                schema: "public",
                table: "Merchants",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_Email",
                schema: "public",
                table: "Merchants",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_MerchantCode",
                schema: "public",
                table: "Merchants",
                column: "MerchantCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_TaxNumber",
                schema: "public",
                table: "Merchants",
                column: "TaxNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_CreatedAt",
                schema: "public",
                table: "Terminals",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_MerchantId",
                schema: "public",
                table: "Terminals",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_StatusId",
                schema: "public",
                table: "Terminals",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_TerminalCode",
                schema: "public",
                table: "Terminals",
                column: "TerminalCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Terminals",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Merchants",
                schema: "public");
        }
    }
}
