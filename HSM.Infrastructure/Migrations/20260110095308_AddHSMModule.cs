using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HSM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHSMModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HSMCommandLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HSMDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommandTypeId = table.Column<int>(type: "int", nullable: false),
                    RequestData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ResponseData = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResponseCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExecutionTimeMs = table.Column<int>(type: "int", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CardNumberMasked = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HSMCommandLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HSMDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceTypeId = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    SecondaryIpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SecondaryPort = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    TimeoutMs = table.Column<int>(type: "int", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    LastHealthCheck = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSuccessfulCommand = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HeaderLength = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HSMDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HSMKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KeyTypeId = table.Column<int>(type: "int", nullable: false),
                    KeyIndex = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EncryptedKeyValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    KeyCheckValue = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    KeyLength = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HSMDeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HSMKeys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HSMCommandLogs_ExecutedAt",
                table: "HSMCommandLogs",
                column: "ExecutedAt");

            migrationBuilder.CreateIndex(
                name: "IX_HSMCommandLogs_HSMDeviceId",
                table: "HSMCommandLogs",
                column: "HSMDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_HSMCommandLogs_IsSuccess",
                table: "HSMCommandLogs",
                column: "IsSuccess");

            migrationBuilder.CreateIndex(
                name: "IX_HSMCommandLogs_ReferenceId",
                table: "HSMCommandLogs",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_HSMDevices_DeviceName",
                table: "HSMDevices",
                column: "DeviceName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HSMDevices_IsActive",
                table: "HSMDevices",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HSMDevices_IsPrimary",
                table: "HSMDevices",
                column: "IsPrimary");

            migrationBuilder.CreateIndex(
                name: "IX_HSMKeys_HSMDeviceId",
                table: "HSMKeys",
                column: "HSMDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_HSMKeys_IsActive",
                table: "HSMKeys",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HSMKeys_KeyIndex",
                table: "HSMKeys",
                column: "KeyIndex");

            migrationBuilder.CreateIndex(
                name: "IX_HSMKeys_KeyName",
                table: "HSMKeys",
                column: "KeyName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HSMCommandLogs");

            migrationBuilder.DropTable(
                name: "HSMDevices");

            migrationBuilder.DropTable(
                name: "HSMKeys");
        }
    }
}
