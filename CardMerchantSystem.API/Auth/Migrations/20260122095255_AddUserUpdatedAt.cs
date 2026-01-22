using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardMerchantSystem.API.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddUserUpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AuthUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "PasswordHash", "UpdatedAt" },
                values: new object[] { "$2a$11$6vFb06U68k1.MUtY6cWpOuFIJ5dalPtWcuCLNkxJlSRec1te7SqpW", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AuthUsers");

            migrationBuilder.UpdateData(
                table: "AuthUsers",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$js811WZYsXJ1mbIojynCm.1vDcxaBagZ0YzVHI8x2gURAXlMf5iMS");
        }
    }
}
