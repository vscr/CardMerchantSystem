using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Fraud.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialFraudModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "fraud");

            migrationBuilder.CreateTable(
                name: "CardFraudProfiles",
                schema: "fraud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaskedCardNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalTransactionCount = table.Column<int>(type: "int", nullable: false),
                    Last1HourTxCount = table.Column<int>(type: "int", nullable: false),
                    Last24HourTxCount = table.Column<int>(type: "int", nullable: false),
                    Last7DayTxCount = table.Column<int>(type: "int", nullable: false),
                    TotalTransactionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Last1HourTxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Last24HourTxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Last24HourDistinctCountryCount = table.Column<int>(type: "int", nullable: false),
                    Last24HourDistinctMerchantCount = table.Column<int>(type: "int", nullable: false),
                    LastTransactionCountry = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    LastMerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TotalHitScenarioCount = table.Column<int>(type: "int", nullable: false),
                    TotalFraudConfirmedCount = table.Column<int>(type: "int", nullable: false),
                    DeclinedTransactionCount = table.Column<int>(type: "int", nullable: false),
                    LastFraudAlertDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentRiskScore = table.Column<int>(type: "int", nullable: false),
                    LastTransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstTransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardFraudProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FraudAlerts",
                schema: "fraud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaskedCardNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TransactionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    HitScenarioCount = table.Column<int>(type: "int", nullable: false),
                    HighestFraudResponseCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNote = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Decision = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FraudBlacklists",
                schema: "fraud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsBlacklist = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudBlacklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FraudRules",
                schema: "fraud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    LogicalOperator = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PeriodMinutes = table.Column<int>(type: "int", nullable: true),
                    PeriodThreshold = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    PeriodFunction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PeriodGroupBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SqlScript = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FraudActions",
                schema: "fraud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FraudAlertId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaskedCardNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CardStatusAction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CardStatusReasonCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActionBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraudActions_FraudAlerts_FraudAlertId",
                        column: x => x.FraudAlertId,
                        principalSchema: "fraud",
                        principalTable: "FraudAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FraudRuleConditions",
                schema: "fraud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FraudRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParameterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Operator = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SecondValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudRuleConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraudRuleConditions_FraudRules_FraudRuleId",
                        column: x => x.FraudRuleId,
                        principalSchema: "fraud",
                        principalTable: "FraudRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FraudScenarios",
                schema: "fraud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScenarioNo = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilterRuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CheckMode = table.Column<int>(type: "int", nullable: false),
                    FraudResponseCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    RunOrder = table.Column<int>(type: "int", nullable: false),
                    IsSimulation = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraudScenarios_FraudRules_FilterRuleId",
                        column: x => x.FilterRuleId,
                        principalSchema: "fraud",
                        principalTable: "FraudRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FraudScenarios_FraudRules_RuleId",
                        column: x => x.RuleId,
                        principalSchema: "fraud",
                        principalTable: "FraudRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HitScenarios",
                schema: "fraud",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FraudScenarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaskedCardNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MerchantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    FraudResponseCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    IsSimulation = table.Column<bool>(type: "bit", nullable: false),
                    ExecutionTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HitScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HitScenarios_FraudScenarios_FraudScenarioId",
                        column: x => x.FraudScenarioId,
                        principalSchema: "fraud",
                        principalTable: "FraudScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "fraud",
                table: "FraudBlacklists",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "ExpiresAt", "IsActive", "IsBlacklist", "ListType", "Reason", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { new Guid("d0000001-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, true, "Country", "Kuzey Kore — yaptırım ülkesi", null, null, "KP" },
                    { new Guid("d0000001-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, true, "Country", "İran — yaptırım ülkesi", null, null, "IR" },
                    { new Guid("d0000001-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, true, "Country", "Suriye — yaptırım ülkesi", null, null, "SY" },
                    { new Guid("d0000001-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, true, "MCC", "Kumar — yüksek riskli MCC", null, null, "7995" },
                    { new Guid("d0000001-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, true, "MCC", "Doğrudan pazarlama — yüksek riskli", null, null, "5967" },
                    { new Guid("d0000001-0000-0000-0000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, true, false, "Country", "Türkiye — beyaz liste (yerli işlem)", null, null, "TR" }
                });

            migrationBuilder.InsertData(
                schema: "fraud",
                table: "FraudRules",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Description", "IsActive", "LogicalOperator", "Name", "PeriodFunction", "PeriodGroupBy", "PeriodMinutes", "PeriodThreshold", "RuleType", "SqlScript", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("a0000001-0000-0000-0000-000000000001"), "R001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Tek işlem tutarı 25.000 TL üzeri", true, 1, "Yüksek Tutarlı İşlem", null, null, null, null, 1, null, null, null },
                    { new Guid("a0000001-0000-0000-0000-000000000002"), "R002", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Yurtdışı işlem + 5.000 TL üzeri", true, 1, "Yurtdışı Yüksek Tutar", null, null, null, null, 2, null, null, null },
                    { new Guid("a0000001-0000-0000-0000-000000000003"), "R003", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "02:00-06:00 arası + 10.000 TL üzeri", true, 1, "Gece Yüksek Tutar", null, null, null, null, 2, null, null, null },
                    { new Guid("a0000001-0000-0000-0000-000000000004"), "R004", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Yüksek riskli üye işyeri kategori kodları", true, 1, "Riskli MCC", null, null, null, null, 1, null, null, null },
                    { new Guid("a0000001-0000-0000-0000-000000000005"), "R005", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Son 1 saatte aynı karttan 5+ işlem", true, 1, "Saatlik Çok İşlem", "COUNT", "CARD", 60, 5m, 3, null, null, null },
                    { new Guid("a0000001-0000-0000-0000-000000000006"), "R006", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Son 24 saatte 50.000 TL+ toplam işlem", true, 1, "Günlük Yüksek Toplam", "SUM", "CARD", 1440, 50000m, 3, null, null, null },
                    { new Guid("a0000001-0000-0000-0000-000000000007"), "R007", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "PIN girilmemiş + 3.000 TL üzeri", true, 1, "PIN'siz Yüksek Tutar", null, null, null, null, 2, null, null, null },
                    { new Guid("a0000001-0000-0000-0000-000000000008"), "F001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "100 TL altı işlemleri hariç tut", true, 1, "Düşük Tutar Filtresi", null, null, null, null, 1, null, null, null }
                });

            migrationBuilder.InsertData(
                schema: "fraud",
                table: "FraudRuleConditions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "FraudRuleId", "Operator", "OrderIndex", "ParameterName", "SecondValue", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[,]
                {
                    { new Guid("b0000001-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000001"), 3, 1, "OriginalAmount", null, null, null, "25000" },
                    { new Guid("b0000001-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000002"), 2, 1, "MerchantCountryCode", null, null, null, "TR" },
                    { new Guid("b0000001-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000002"), 3, 2, "OriginalAmount", null, null, null, "5000" },
                    { new Guid("b0000001-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000003"), 4, 1, "TransactionHour", null, null, null, "2" },
                    { new Guid("b0000001-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000003"), 6, 2, "TransactionHour", null, null, null, "6" },
                    { new Guid("b0000001-0000-0000-0000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000003"), 3, 3, "OriginalAmount", null, null, null, "10000" },
                    { new Guid("b0000001-0000-0000-0000-000000000007"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000004"), 9, 1, "Mcc", null, null, null, "6051,6211,7995,5967,5966" },
                    { new Guid("b0000001-0000-0000-0000-000000000008"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000007"), 1, 1, "IsPinEntered", null, null, null, "False" },
                    { new Guid("b0000001-0000-0000-0000-000000000009"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000007"), 3, 2, "OriginalAmount", null, null, null, "3000" },
                    { new Guid("b0000001-0000-0000-0000-000000000010"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("a0000001-0000-0000-0000-000000000008"), 5, 1, "OriginalAmount", null, null, null, "100" }
                });

            migrationBuilder.InsertData(
                schema: "fraud",
                table: "FraudScenarios",
                columns: new[] { "Id", "CheckMode", "CreatedAt", "CreatedBy", "Description", "EndDate", "FilterRuleId", "FraudResponseCode", "IsActive", "IsSimulation", "Name", "RuleId", "RunOrder", "ScenarioNo", "Score", "StartDate", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("c0000001-0000-0000-0000-000000000001"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "25.000 TL üzeri tek işlem kontrolü", null, new Guid("a0000001-0000-0000-0000-000000000008"), "05", true, false, "Yüksek Tutarlı İşlem Tespiti", new Guid("a0000001-0000-0000-0000-000000000001"), 1, 1, 70, null, null, null },
                    { new Guid("c0000001-0000-0000-0000-000000000002"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Yurtdışı + yüksek tutar kombinasyonu", null, null, "01", true, false, "Yurtdışı Şüpheli İşlem", new Guid("a0000001-0000-0000-0000-000000000002"), 2, 2, 85, null, null, null },
                    { new Guid("c0000001-0000-0000-0000-000000000003"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "02-06 arası yüksek tutarlı işlem", null, null, "01", true, false, "Gece Saati Yüksek Tutar", new Guid("a0000001-0000-0000-0000-000000000003"), 3, 3, 80, null, null, null },
                    { new Guid("c0000001-0000-0000-0000-000000000004"), 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Yüksek riskli MCC kodlarında işlem", null, new Guid("a0000001-0000-0000-0000-000000000008"), "01", true, true, "Riskli Kategori İşlemi", new Guid("a0000001-0000-0000-0000-000000000004"), 4, 4, 60, null, null, null },
                    { new Guid("c0000001-0000-0000-0000-000000000005"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "Kısa sürede çok fazla işlem", null, null, "05", true, false, "Sıklık Anomalisi", new Guid("a0000001-0000-0000-0000-000000000005"), 5, 5, 90, null, null, null },
                    { new Guid("c0000001-0000-0000-0000-000000000006"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "24 saatte 50K+ toplam işlem", null, null, "01", true, false, "Günlük Limit Aşımı", new Guid("a0000001-0000-0000-0000-000000000006"), 6, 6, 75, null, null, null },
                    { new Guid("c0000001-0000-0000-0000-000000000007"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", "PIN doğrulaması olmadan yüksek tutar", null, null, "01", true, false, "PIN'siz Yüksek Tutar", new Guid("a0000001-0000-0000-0000-000000000007"), 7, 7, 65, null, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardFraudProfiles_CurrentRiskScore",
                schema: "fraud",
                table: "CardFraudProfiles",
                column: "CurrentRiskScore");

            migrationBuilder.CreateIndex(
                name: "IX_CardFraudProfiles_MaskedCardNo",
                schema: "fraud",
                table: "CardFraudProfiles",
                column: "MaskedCardNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FraudActions_FraudAlertId",
                schema: "fraud",
                table: "FraudActions",
                column: "FraudAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudActions_MaskedCardNo",
                schema: "fraud",
                table: "FraudActions",
                column: "MaskedCardNo");

            migrationBuilder.CreateIndex(
                name: "IX_FraudActions_TransactionId",
                schema: "fraud",
                table: "FraudActions",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_AssignedTo",
                schema: "fraud",
                table: "FraudAlerts",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_CreatedAt",
                schema: "fraud",
                table: "FraudAlerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_MaskedCardNo",
                schema: "fraud",
                table: "FraudAlerts",
                column: "MaskedCardNo");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_Status",
                schema: "fraud",
                table: "FraudAlerts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_TransactionId",
                schema: "fraud",
                table: "FraudAlerts",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FraudBlacklists_ListType_IsBlacklist_IsActive",
                schema: "fraud",
                table: "FraudBlacklists",
                columns: new[] { "ListType", "IsBlacklist", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_FraudBlacklists_ListType_Value",
                schema: "fraud",
                table: "FraudBlacklists",
                columns: new[] { "ListType", "Value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FraudRuleConditions_FraudRuleId",
                schema: "fraud",
                table: "FraudRuleConditions",
                column: "FraudRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudRules_Code",
                schema: "fraud",
                table: "FraudRules",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FraudRules_IsActive",
                schema: "fraud",
                table: "FraudRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FraudScenarios_FilterRuleId",
                schema: "fraud",
                table: "FraudScenarios",
                column: "FilterRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudScenarios_IsActive",
                schema: "fraud",
                table: "FraudScenarios",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_FraudScenarios_IsActive_CheckMode",
                schema: "fraud",
                table: "FraudScenarios",
                columns: new[] { "IsActive", "CheckMode" });

            migrationBuilder.CreateIndex(
                name: "IX_FraudScenarios_RuleId",
                schema: "fraud",
                table: "FraudScenarios",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudScenarios_ScenarioNo",
                schema: "fraud",
                table: "FraudScenarios",
                column: "ScenarioNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HitScenarios_DetectedAt",
                schema: "fraud",
                table: "HitScenarios",
                column: "DetectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_HitScenarios_FraudScenarioId",
                schema: "fraud",
                table: "HitScenarios",
                column: "FraudScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HitScenarios_MaskedCardNo",
                schema: "fraud",
                table: "HitScenarios",
                column: "MaskedCardNo");

            migrationBuilder.CreateIndex(
                name: "IX_HitScenarios_TransactionId",
                schema: "fraud",
                table: "HitScenarios",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardFraudProfiles",
                schema: "fraud");

            migrationBuilder.DropTable(
                name: "FraudActions",
                schema: "fraud");

            migrationBuilder.DropTable(
                name: "FraudBlacklists",
                schema: "fraud");

            migrationBuilder.DropTable(
                name: "FraudRuleConditions",
                schema: "fraud");

            migrationBuilder.DropTable(
                name: "HitScenarios",
                schema: "fraud");

            migrationBuilder.DropTable(
                name: "FraudAlerts",
                schema: "fraud");

            migrationBuilder.DropTable(
                name: "FraudScenarios",
                schema: "fraud");

            migrationBuilder.DropTable(
                name: "FraudRules",
                schema: "fraud");
        }
    }
}
