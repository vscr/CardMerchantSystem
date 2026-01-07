using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerTckn = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomerSurname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BuildingNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ApartmentNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CardTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CardNumberEncrypted = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    DailyLimitAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DailyLimitCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    MonthlyLimitAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyLimitCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PrintVendorId = table.Column<int>(type: "int", nullable: true),
                    PrintBatchId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PrintedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourierTrackingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RejectedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardApplicationStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardApplicationStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardApplicationStatusHistories_CardApplications_CardApplicationId",
                        column: x => x.CardApplicationId,
                        principalTable: "CardApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardApplications_CreatedAt",
                table: "CardApplications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CardApplications_CustomerTckn",
                table: "CardApplications",
                column: "CustomerTckn");

            migrationBuilder.CreateIndex(
                name: "IX_CardApplications_Email",
                table: "CardApplications",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_CardApplicationStatusHistories_CardApplicationId",
                table: "CardApplicationStatusHistories",
                column: "CardApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_CardApplicationStatusHistories_ChangedAt",
                table: "CardApplicationStatusHistories",
                column: "ChangedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardApplicationStatusHistories");

            migrationBuilder.DropTable(
                name: "CardApplications");
        }
    }
}
