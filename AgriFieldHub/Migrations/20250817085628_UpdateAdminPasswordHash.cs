using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace AgriFieldHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash" },
                values: new object[] { "p9pY0i4lT3b2kWmQp7sQ2w==:b4W7k4j7nZ8R5kQ1l6k+o7uXv6gJ2x5tqS3Tg8yG5lI=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash" },
                values: new object[] { "hashed" });
        }
    }
}
