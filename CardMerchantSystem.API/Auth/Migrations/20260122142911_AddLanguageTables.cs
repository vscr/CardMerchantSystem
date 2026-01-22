using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CardMerchantSystem.API.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LanguageEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Languages_LanguageEntityId",
                        column: x => x.LanguageEntityId,
                        principalTable: "Languages",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$933PXijjtQ1qhyz1gyxAOub8lC.TfyOgvLT1CpsPCOp70e7kq5cIa");

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "CreatedAt", "IsActive", "IsDefault", "Name", "NativeName" },
                values: new object[,]
                {
                    { 1, "tr", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, "Turkish", "Türkçe" },
                    { 2, "en", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, "English", "English" }
                });

            migrationBuilder.InsertData(
                table: "Translations",
                columns: new[] { "Id", "Category", "CreatedAt", "Key", "LanguageCode", "LanguageEntityId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0001-000000000001"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.dashboard", "tr", null, null, "Dashboard" },
                    { new Guid("40000000-0000-0000-0001-000000000002"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.card-management", "tr", null, null, "Kart Yönetimi" },
                    { new Guid("40000000-0000-0000-0001-000000000003"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.cards", "tr", null, null, "Kartlar" },
                    { new Guid("40000000-0000-0000-0001-000000000004"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.card-applications", "tr", null, null, "Başvurular" },
                    { new Guid("40000000-0000-0000-0001-000000000005"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.card-blocks", "tr", null, null, "Blokeler" },
                    { new Guid("40000000-0000-0000-0001-000000000006"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.merchant-management", "tr", null, null, "Üye İşyeri" },
                    { new Guid("40000000-0000-0000-0001-000000000007"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.merchants", "tr", null, null, "İşyerleri" },
                    { new Guid("40000000-0000-0000-0001-000000000008"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.terminals", "tr", null, null, "Terminaller" },
                    { new Guid("40000000-0000-0000-0001-000000000009"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.finance-management", "tr", null, null, "Finans" },
                    { new Guid("40000000-0000-0000-0001-000000000010"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.transactions", "tr", null, null, "İşlemler" },
                    { new Guid("40000000-0000-0000-0001-000000000011"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.statements", "tr", null, null, "Ekstreler" },
                    { new Guid("40000000-0000-0000-0001-000000000012"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.accounting", "tr", null, null, "Muhasebe" },
                    { new Guid("40000000-0000-0000-0001-000000000013"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.work-orders", "tr", null, null, "İş Emirleri" },
                    { new Guid("40000000-0000-0000-0001-000000000014"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.system-management", "tr", null, null, "Sistem" },
                    { new Guid("40000000-0000-0000-0001-000000000015"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.users", "tr", null, null, "Kullanıcılar" },
                    { new Guid("40000000-0000-0000-0001-000000000016"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.roles", "tr", null, null, "Roller" },
                    { new Guid("40000000-0000-0000-0001-000000000017"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.menus", "tr", null, null, "Menüler" },
                    { new Guid("40000000-0000-0000-0002-000000000001"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.dashboard", "en", null, null, "Dashboard" },
                    { new Guid("40000000-0000-0000-0002-000000000002"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.card-management", "en", null, null, "Card Management" },
                    { new Guid("40000000-0000-0000-0002-000000000003"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.cards", "en", null, null, "Cards" },
                    { new Guid("40000000-0000-0000-0002-000000000004"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.card-applications", "en", null, null, "Applications" },
                    { new Guid("40000000-0000-0000-0002-000000000005"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.card-blocks", "en", null, null, "Blocks" },
                    { new Guid("40000000-0000-0000-0002-000000000006"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.merchant-management", "en", null, null, "Merchant" },
                    { new Guid("40000000-0000-0000-0002-000000000007"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.merchants", "en", null, null, "Merchants" },
                    { new Guid("40000000-0000-0000-0002-000000000008"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.terminals", "en", null, null, "Terminals" },
                    { new Guid("40000000-0000-0000-0002-000000000009"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.finance-management", "en", null, null, "Finance" },
                    { new Guid("40000000-0000-0000-0002-000000000010"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.transactions", "en", null, null, "Transactions" },
                    { new Guid("40000000-0000-0000-0002-000000000011"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.statements", "en", null, null, "Statements" },
                    { new Guid("40000000-0000-0000-0002-000000000012"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.accounting", "en", null, null, "Accounting" },
                    { new Guid("40000000-0000-0000-0002-000000000013"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.work-orders", "en", null, null, "Work Orders" },
                    { new Guid("40000000-0000-0000-0002-000000000014"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.system-management", "en", null, null, "System" },
                    { new Guid("40000000-0000-0000-0002-000000000015"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.users", "en", null, null, "Users" },
                    { new Guid("40000000-0000-0000-0002-000000000016"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.roles", "en", null, null, "Roles" },
                    { new Guid("40000000-0000-0000-0002-000000000017"), "menu", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "menu.menus", "en", null, null, "Menus" },
                    { new Guid("40000000-0000-0000-0003-000000000001"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.Admin", "tr", null, null, "Sistem Yöneticisi" },
                    { new Guid("40000000-0000-0000-0003-000000000002"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.CardOperator", "tr", null, null, "Kart Operasyon" },
                    { new Guid("40000000-0000-0000-0003-000000000003"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.MerchantOperator", "tr", null, null, "Üye İşyeri Operasyon" },
                    { new Guid("40000000-0000-0000-0003-000000000004"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.FinanceOperator", "tr", null, null, "Finans Operasyon" },
                    { new Guid("40000000-0000-0000-0003-000000000005"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.ComplianceOfficer", "tr", null, null, "Uyum Sorumlusu" },
                    { new Guid("40000000-0000-0000-0003-000000000006"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.CallCenterAgent", "tr", null, null, "Çağrı Merkezi" },
                    { new Guid("40000000-0000-0000-0003-000000000007"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.Viewer", "tr", null, null, "Görüntüleyici" },
                    { new Guid("40000000-0000-0000-0004-000000000001"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.Admin", "en", null, null, "System Administrator" },
                    { new Guid("40000000-0000-0000-0004-000000000002"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.CardOperator", "en", null, null, "Card Operator" },
                    { new Guid("40000000-0000-0000-0004-000000000003"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.MerchantOperator", "en", null, null, "Merchant Operator" },
                    { new Guid("40000000-0000-0000-0004-000000000004"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.FinanceOperator", "en", null, null, "Finance Operator" },
                    { new Guid("40000000-0000-0000-0004-000000000005"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.ComplianceOfficer", "en", null, null, "Compliance Officer" },
                    { new Guid("40000000-0000-0000-0004-000000000006"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.CallCenterAgent", "en", null, null, "Call Center Agent" },
                    { new Guid("40000000-0000-0000-0004-000000000007"), "role", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "role.Viewer", "en", null, null, "Viewer" },
                    { new Guid("40000000-0000-0000-0005-000000000001"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.save", "tr", null, null, "Kaydet" },
                    { new Guid("40000000-0000-0000-0005-000000000002"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.cancel", "tr", null, null, "İptal" },
                    { new Guid("40000000-0000-0000-0005-000000000003"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.delete", "tr", null, null, "Sil" },
                    { new Guid("40000000-0000-0000-0005-000000000004"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.edit", "tr", null, null, "Düzenle" },
                    { new Guid("40000000-0000-0000-0005-000000000005"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.add", "tr", null, null, "Ekle" },
                    { new Guid("40000000-0000-0000-0005-000000000006"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.search", "tr", null, null, "Ara" },
                    { new Guid("40000000-0000-0000-0005-000000000007"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.filter", "tr", null, null, "Filtrele" },
                    { new Guid("40000000-0000-0000-0005-000000000008"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.refresh", "tr", null, null, "Yenile" },
                    { new Guid("40000000-0000-0000-0005-000000000009"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.yes", "tr", null, null, "Evet" },
                    { new Guid("40000000-0000-0000-0005-000000000010"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.no", "tr", null, null, "Hayır" },
                    { new Guid("40000000-0000-0000-0005-000000000011"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.loading", "tr", null, null, "Yükleniyor..." },
                    { new Guid("40000000-0000-0000-0005-000000000012"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.noData", "tr", null, null, "Veri bulunamadı" },
                    { new Guid("40000000-0000-0000-0005-000000000013"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.success", "tr", null, null, "Başarılı" },
                    { new Guid("40000000-0000-0000-0005-000000000014"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.error", "tr", null, null, "Hata" },
                    { new Guid("40000000-0000-0000-0005-000000000015"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.warning", "tr", null, null, "Uyarı" },
                    { new Guid("40000000-0000-0000-0005-000000000016"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.confirm", "tr", null, null, "Onayla" },
                    { new Guid("40000000-0000-0000-0005-000000000017"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.back", "tr", null, null, "Geri" },
                    { new Guid("40000000-0000-0000-0005-000000000018"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.next", "tr", null, null, "İleri" },
                    { new Guid("40000000-0000-0000-0005-000000000019"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.close", "tr", null, null, "Kapat" },
                    { new Guid("40000000-0000-0000-0005-000000000020"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.actions", "tr", null, null, "İşlemler" },
                    { new Guid("40000000-0000-0000-0006-000000000001"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.save", "en", null, null, "Save" },
                    { new Guid("40000000-0000-0000-0006-000000000002"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.cancel", "en", null, null, "Cancel" },
                    { new Guid("40000000-0000-0000-0006-000000000003"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.delete", "en", null, null, "Delete" },
                    { new Guid("40000000-0000-0000-0006-000000000004"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.edit", "en", null, null, "Edit" },
                    { new Guid("40000000-0000-0000-0006-000000000005"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.add", "en", null, null, "Add" },
                    { new Guid("40000000-0000-0000-0006-000000000006"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.search", "en", null, null, "Search" },
                    { new Guid("40000000-0000-0000-0006-000000000007"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.filter", "en", null, null, "Filter" },
                    { new Guid("40000000-0000-0000-0006-000000000008"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.refresh", "en", null, null, "Refresh" },
                    { new Guid("40000000-0000-0000-0006-000000000009"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.yes", "en", null, null, "Yes" },
                    { new Guid("40000000-0000-0000-0006-000000000010"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.no", "en", null, null, "No" },
                    { new Guid("40000000-0000-0000-0006-000000000011"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.loading", "en", null, null, "Loading..." },
                    { new Guid("40000000-0000-0000-0006-000000000012"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.noData", "en", null, null, "No data found" },
                    { new Guid("40000000-0000-0000-0006-000000000013"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.success", "en", null, null, "Success" },
                    { new Guid("40000000-0000-0000-0006-000000000014"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.error", "en", null, null, "Error" },
                    { new Guid("40000000-0000-0000-0006-000000000015"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.warning", "en", null, null, "Warning" },
                    { new Guid("40000000-0000-0000-0006-000000000016"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.confirm", "en", null, null, "Confirm" },
                    { new Guid("40000000-0000-0000-0006-000000000017"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.back", "en", null, null, "Back" },
                    { new Guid("40000000-0000-0000-0006-000000000018"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.next", "en", null, null, "Next" },
                    { new Guid("40000000-0000-0000-0006-000000000019"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.close", "en", null, null, "Close" },
                    { new Guid("40000000-0000-0000-0006-000000000020"), "common", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "common.actions", "en", null, null, "Actions" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Code",
                table: "Languages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_Category",
                table: "Translations",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageCode_Key",
                table: "Translations",
                columns: new[] { "LanguageCode", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageEntityId",
                table: "Translations",
                column: "LanguageEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translations");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$6vFb06U68k1.MUtY6cWpOuFIJ5dalPtWcuCLNkxJlSRec1te7SqpW");
        }
    }
}
