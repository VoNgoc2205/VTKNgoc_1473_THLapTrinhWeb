using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1473_VTKNgoc_Buoi3.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
            values: new object[,]
            {
                { "role-admin", "Admin", "ADMIN", "role-admin-stamp" },
                { "role-user", "User", "USER", "role-user-stamp" }
            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
