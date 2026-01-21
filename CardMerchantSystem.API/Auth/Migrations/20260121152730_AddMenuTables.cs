using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CardMerchantSystem.API.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthMenus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Path = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthMenus_AuthMenus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AuthMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthMenuClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthMenuClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthMenuClaims_AuthMenus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "AuthMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AuthMenus",
                columns: new[] { "Id", "CreatedAt", "DisplayOrder", "Icon", "IsActive", "IsVisible", "Name", "ParentId", "Path", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "layout-dashboard", true, true, "dashboard", null, "/dashboard", "Dashboard", null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "credit-card", true, true, "card-management", null, null, "Kart Yönetimi", null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "store", true, true, "merchant-management", null, null, "Üye İşyeri", null },
                    { new Guid("10000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, "wallet", true, true, "finance-management", null, null, "Finans", null },
                    { new Guid("10000000-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, "clipboard-list", true, true, "work-orders", null, "/work-orders", "İş Emirleri", null },
                    { new Guid("10000000-0000-0000-0000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, "settings", true, true, "system-management", null, null, "Sistem", null }
                });

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$js811WZYsXJ1mbIojynCm.1vDcxaBagZ0YzVHI8x2gURAXlMf5iMS");

            migrationBuilder.InsertData(
                table: "AuthMenuClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "MenuId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), "Policy", "ViewerOrAbove", new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "Policy", "CardManagement", new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000006"), "Policy", "MerchantManagement", new Guid("10000000-0000-0000-0000-000000000003") },
                    { new Guid("30000000-0000-0000-0000-000000000009"), "Policy", "FinanceManagement", new Guid("10000000-0000-0000-0000-000000000004") },
                    { new Guid("30000000-0000-0000-0000-000000000013"), "Policy", "WorkOrderManagement", new Guid("10000000-0000-0000-0000-000000000005") },
                    { new Guid("30000000-0000-0000-0000-000000000014"), "Policy", "AdminOnly", new Guid("10000000-0000-0000-0000-000000000006") }
                });

            migrationBuilder.InsertData(
                table: "AuthMenus",
                columns: new[] { "Id", "CreatedAt", "DisplayOrder", "Icon", "IsActive", "IsVisible", "Name", "ParentId", "Path", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "credit-card", true, true, "cards", new Guid("10000000-0000-0000-0000-000000000002"), "/cards", "Kartlar", null },
                    { new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "file-text", true, true, "card-applications", new Guid("10000000-0000-0000-0000-000000000002"), "/card-applications", "Başvurular", null },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "shield-off", true, true, "card-blocks", new Guid("10000000-0000-0000-0000-000000000002"), "/card-blocks", "Blokeler", null },
                    { new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "building", true, true, "merchants", new Guid("10000000-0000-0000-0000-000000000003"), "/merchants", "İşyerleri", null },
                    { new Guid("20000000-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "monitor", true, true, "terminals", new Guid("10000000-0000-0000-0000-000000000003"), "/terminals", "Terminaller", null },
                    { new Guid("20000000-0000-0000-0000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "arrow-left-right", true, true, "transactions", new Guid("10000000-0000-0000-0000-000000000004"), "/transactions", "İşlemler", null },
                    { new Guid("20000000-0000-0000-0000-000000000007"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "file-spreadsheet", true, true, "statements", new Guid("10000000-0000-0000-0000-000000000004"), "/statements", "Ekstreler", null },
                    { new Guid("20000000-0000-0000-0000-000000000008"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "calculator", true, true, "accounting", new Guid("10000000-0000-0000-0000-000000000004"), "/accounting", "Muhasebe", null },
                    { new Guid("20000000-0000-0000-0000-000000000009"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "users", true, true, "users", new Guid("10000000-0000-0000-0000-000000000006"), "/users", "Kullanıcılar", null },
                    { new Guid("20000000-0000-0000-0000-000000000010"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "shield", true, true, "roles", new Guid("10000000-0000-0000-0000-000000000006"), "/roles", "Roller", null },
                    { new Guid("20000000-0000-0000-0000-000000000011"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "menu", true, true, "menus", new Guid("10000000-0000-0000-0000-000000000006"), "/menus", "Menüler", null }
                });

            migrationBuilder.InsertData(
                table: "AuthMenuClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "MenuId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000003"), "Policy", "CardManagement", new Guid("20000000-0000-0000-0000-000000000001") },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "Policy", "CardManagement", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000005"), "Policy", "CardManagement", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("30000000-0000-0000-0000-000000000007"), "Policy", "MerchantManagement", new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("30000000-0000-0000-0000-000000000008"), "Policy", "MerchantManagement", new Guid("20000000-0000-0000-0000-000000000005") },
                    { new Guid("30000000-0000-0000-0000-000000000010"), "Policy", "FinanceManagement", new Guid("20000000-0000-0000-0000-000000000006") },
                    { new Guid("30000000-0000-0000-0000-000000000011"), "Policy", "FinanceManagement", new Guid("20000000-0000-0000-0000-000000000007") },
                    { new Guid("30000000-0000-0000-0000-000000000012"), "Policy", "FinanceManagement", new Guid("20000000-0000-0000-0000-000000000008") },
                    { new Guid("30000000-0000-0000-0000-000000000015"), "Policy", "AdminOnly", new Guid("20000000-0000-0000-0000-000000000009") },
                    { new Guid("30000000-0000-0000-0000-000000000016"), "Policy", "AdminOnly", new Guid("20000000-0000-0000-0000-000000000010") },
                    { new Guid("30000000-0000-0000-0000-000000000017"), "Policy", "AdminOnly", new Guid("20000000-0000-0000-0000-000000000011") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthMenuClaims_MenuId_ClaimType_ClaimValue",
                table: "AuthMenuClaims",
                columns: new[] { "MenuId", "ClaimType", "ClaimValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthMenus_Name",
                table: "AuthMenus",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthMenus_ParentId",
                table: "AuthMenus",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthMenuClaims");

            migrationBuilder.DropTable(
                name: "AuthMenus");

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$Z7KS61.O8dmAgNBto1VyROa0jVoe5qlm2RVOj2wEpMdAXzY.aVoMW");
        }
    }
}
