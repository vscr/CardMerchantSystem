using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fee.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFeeModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommissionBreakdowns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCommission = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BankShare = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InterchangeFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BKMFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MerchantDiscount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    BankShareRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    InterchangeRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    BKMRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    MerchantNetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MCC = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    InstallmentCount = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TariffId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionBreakdowns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeeAccruals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccrualNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FeeTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CardNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    TerminalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GrossAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AccrualDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccrualPeriodStart = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    AccrualPeriodEnd = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TariffId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeAccruals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MembershipFees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FeeTypeId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: false),
                    GracePeriodDays = table.Column<int>(type: "int", nullable: false),
                    LateFeeRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinimumTransactionVolume = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MinimumTransactionCount = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipFees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MerchantTariffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TariffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeeTypeId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SpecialRate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantTariffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tariffs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TariffCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TariffName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FeeTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TariffRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TariffId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CalculationTypeId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    MinimumFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MaximumFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MCC = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    InstallmentCount = table.Column<int>(type: "int", nullable: true),
                    VolumeFrom = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    VolumeTo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TariffRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TariffRules_Tariffs_TariffId",
                        column: x => x.TariffId,
                        principalTable: "Tariffs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommissionBreakdowns_MerchantId",
                table: "CommissionBreakdowns",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionBreakdowns_TransactionDate",
                table: "CommissionBreakdowns",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionBreakdowns_TransactionId",
                table: "CommissionBreakdowns",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeAccruals_AccrualNumber",
                table: "FeeAccruals",
                column: "AccrualNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeAccruals_CardNumber",
                table: "FeeAccruals",
                column: "CardNumber");

            migrationBuilder.CreateIndex(
                name: "IX_FeeAccruals_DueDate",
                table: "FeeAccruals",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_FeeAccruals_MerchantId",
                table: "FeeAccruals",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeAccruals_StatusId",
                table: "FeeAccruals",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeAccruals_TerminalId",
                table: "FeeAccruals",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipFees_FeeName",
                table: "MembershipFees",
                column: "FeeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipFees_FeeTypeId",
                table: "MembershipFees",
                column: "FeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipFees_IsActive",
                table: "MembershipFees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantTariffs_IsActive",
                table: "MerchantTariffs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantTariffs_MerchantId",
                table: "MerchantTariffs",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantTariffs_MerchantId_FeeTypeId_IsActive",
                table: "MerchantTariffs",
                columns: new[] { "MerchantId", "FeeTypeId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_MerchantTariffs_TariffId",
                table: "MerchantTariffs",
                column: "TariffId");

            migrationBuilder.CreateIndex(
                name: "IX_TariffRules_InstallmentCount",
                table: "TariffRules",
                column: "InstallmentCount");

            migrationBuilder.CreateIndex(
                name: "IX_TariffRules_IsActive",
                table: "TariffRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TariffRules_MCC",
                table: "TariffRules",
                column: "MCC");

            migrationBuilder.CreateIndex(
                name: "IX_TariffRules_TariffId",
                table: "TariffRules",
                column: "TariffId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariffs_EffectiveFrom",
                table: "Tariffs",
                column: "EffectiveFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Tariffs_FeeTypeId",
                table: "Tariffs",
                column: "FeeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tariffs_IsDefault",
                table: "Tariffs",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Tariffs_TariffCode",
                table: "Tariffs",
                column: "TariffCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommissionBreakdowns");

            migrationBuilder.DropTable(
                name: "FeeAccruals");

            migrationBuilder.DropTable(
                name: "MembershipFees");

            migrationBuilder.DropTable(
                name: "MerchantTariffs");

            migrationBuilder.DropTable(
                name: "TariffRules");

            migrationBuilder.DropTable(
                name: "Tariffs");
        }
    }
}
