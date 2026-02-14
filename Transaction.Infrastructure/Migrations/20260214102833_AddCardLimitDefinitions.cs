using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transaction.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCardLimitDefinitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CardLimitDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LimitType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TargetValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DailyLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MonthlyLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SingleTransactionLimit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardLimitDefinitions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CardLimitDefinitions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Currency", "DailyLimit", "Description", "IsActive", "LimitType", "MonthlyLimit", "SingleTransactionLimit", "TargetValue", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("e0000001-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "TRY", 10000m, "Sistem varsayılan limiti", true, "DEFAULT", 50000m, 5000m, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_CardLimitDefinitions_IsActive",
                table: "CardLimitDefinitions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CardLimitDefinitions_LimitType_TargetValue",
                table: "CardLimitDefinitions",
                columns: new[] { "LimitType", "TargetValue" },
                unique: true,
                filter: "[TargetValue] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardLimitDefinitions");
        }
    }
}
