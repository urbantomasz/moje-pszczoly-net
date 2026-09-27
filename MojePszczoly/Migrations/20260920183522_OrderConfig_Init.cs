using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MojePszczoly.Migrations
{
    /// <inheritdoc />
    public partial class OrderConfig_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomOrderDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomOrderDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderFormSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UseCustomDates = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NoticeEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NoticeTitle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    NoticeMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFormSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "OrderFormSettings",
                columns: new[] { "Id", "NoticeMessage", "NoticeTitle", "UpdatedAt" },
                values: new object[] { 1, null, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_CustomOrderDates_DateUtc",
                table: "CustomOrderDates",
                column: "DateUtc",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomOrderDates");

            migrationBuilder.DropTable(
                name: "OrderFormSettings");
        }
    }
}
