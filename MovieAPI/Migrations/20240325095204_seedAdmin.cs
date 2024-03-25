using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8535b04c-f32e-49e4-8f83-be56e23f5725");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aa5d54dd-5cee-4f63-ba3a-6a027930bb04");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "aea602ba-3427-407e-ab94-01e215874eba", null, "User", "USER" },
                    { "c7e7861f-ac20-4ca1-a5fb-f8f6c509c153", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c1f45bb9-c978-404f-9da9-5e971fbe242e", 0, "f5cf3b49-094e-4172-8d92-a73da79608f8", "admin@gmail.com", false, false, null, "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEEK4wUmdyDkP6kcqEtOzR00JVsOyl3m28f4eLHzcAkxASTZOXJjKDUcxjwgxmzEIIg==", null, false, "8b11b507-7b57-47eb-a0e5-092235cb7fbe", false, "admin@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "c7e7861f-ac20-4ca1-a5fb-f8f6c509c153", "c1f45bb9-c978-404f-9da9-5e971fbe242e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aea602ba-3427-407e-ab94-01e215874eba");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "c7e7861f-ac20-4ca1-a5fb-f8f6c509c153", "c1f45bb9-c978-404f-9da9-5e971fbe242e" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c7e7861f-ac20-4ca1-a5fb-f8f6c509c153");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c1f45bb9-c978-404f-9da9-5e971fbe242e");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8535b04c-f32e-49e4-8f83-be56e23f5725", null, "User", "USER" },
                    { "aa5d54dd-5cee-4f63-ba3a-6a027930bb04", null, "Admin", "ADMIN" }
                });
        }
    }
}
