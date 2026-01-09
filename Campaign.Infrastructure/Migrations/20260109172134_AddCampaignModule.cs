using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Campaign.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CampaignTypeId = table.Column<int>(type: "int", nullable: false),
                    DiscountTypeId = table.Column<int>(type: "int", nullable: false),
                    TargetAudienceId = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    MinTransactionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PointsMultiplier = table.Column<int>(type: "int", nullable: false),
                    TotalBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    UsedBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxUsageCount = table.Column<int>(type: "int", nullable: true),
                    CurrentUsageCount = table.Column<int>(type: "int", nullable: false),
                    MaxUsagePerCustomer = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAllMerchants = table.Column<bool>(type: "bit", nullable: false),
                    AllowedMerchantIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CampaignRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RuleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignRules_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CampaignUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    MerchantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MerchantCode = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    OriginalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CampaignUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignUsages_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignRules_CampaignId",
                table: "CampaignRules",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CampaignCode",
                table: "Campaigns",
                column: "CampaignCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CreatedAt",
                table: "Campaigns",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_EndDate",
                table: "Campaigns",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_StartDate",
                table: "Campaigns",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignUsages_CampaignId",
                table: "CampaignUsages",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignUsages_CardNumberMasked",
                table: "CampaignUsages",
                column: "CardNumberMasked");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignUsages_MerchantId",
                table: "CampaignUsages",
                column: "MerchantId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignUsages_TransactionId",
                table: "CampaignUsages",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignUsages_UsedAt",
                table: "CampaignUsages",
                column: "UsedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CampaignRules");

            migrationBuilder.DropTable(
                name: "CampaignUsages");

            migrationBuilder.DropTable(
                name: "Campaigns");
        }
    }
}
