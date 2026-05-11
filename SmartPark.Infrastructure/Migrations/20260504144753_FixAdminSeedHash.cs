using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPark.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAdminSeedHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "PasswordHash", "Role" },
                values: new object[] { "$2a$11$mC7GdB9mSsm.7pC8hscBdeR4f6UtyW6H.88M87S66pX.Y72v5w123", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "PasswordHash", "Role" },
                values: new object[] { "$2a$11$rFSmt1fKwsRUV1tjuNZyOuGXOsDKU4.5Fxeab5eJ0MqEhMrYSLcIG", "admin" });
        }
    }
}
