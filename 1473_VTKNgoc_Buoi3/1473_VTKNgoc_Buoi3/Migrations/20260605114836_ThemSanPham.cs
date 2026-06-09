using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1473_VTKNgoc_Buoi3.Migrations
{
    /// <inheritdoc />
    public partial class ThemSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Thức ăn thú cưng" },
                    { 2, "Phụ kiện" },
                    { 3, "Chăm sóc sức khỏe" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Hạt giàu protein, bổ sung vitamin giúp thú cưng khỏe mạnh.", "/images/Hat.jpg", "Hạt dinh dưỡng cho thú cưng", 185000m },
                    { 2, 2, "Gối ngủ mềm mại, thoải mái cho chó mèo nghỉ ngơi.", "/images/GoiNgu.jpg", "Gối ngủ thú cưng", 249000m },
                    { 3, 2, "Nhà cây leo trèo, vui chơi và nghỉ ngơi cho mèo.", "/images/Cat.jpg", "Nhà cây cho mèo", 390000m }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
