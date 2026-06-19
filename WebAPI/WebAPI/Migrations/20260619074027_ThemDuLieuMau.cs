using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ThemDuLieuMau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
      table: "Categories",
      columns: new[] { "Id", "Name" },
      values: new object[,]
      {
            { 1, "Thức ăn" },
            { 2, "Phụ kiện" }
      });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[]
                {
            "Id", "Name", "Price", "Description", "ImageUrl", "CategoryId"
                },
                values: new object[,]
                {
            { 1, "Pate mèo", 25000m, "Pate dinh dưỡng cho mèo", "pate.jpg", 1 },
            { 2, "Cát vệ sinh", 80000m, "Cát vệ sinh cho mèo", "cat.jpg", 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
        table: "Products",
        keyColumn: "Id",
        keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2 });
        }
    }
}
