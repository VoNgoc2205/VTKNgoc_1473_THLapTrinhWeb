using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1473_VTKNgoc_Buoi3.Migrations
{
    /// <inheritdoc />
    public partial class ThemDichVu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[] { 4, "Dịch vụ chăm sóc" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 4, 4, "Dịch vụ tắm, sấy và cắt tỉa lông giúp thú cưng sạch sẽ, thơm tho.", "/images/tam.jpg", "Spa & Grooming cao cấp", 200000m },
                    { 5, 4, "Không gian lưu trú sạch sẽ, có khu vui chơi và nhân viên theo dõi thú cưng.", "/images/kstc.jpg", "Khách sạn cho thú cưng", 350000m },
                    { 6, 4, "Gói kiểm tra sức khỏe định kỳ và tư vấn tiêm phòng cần thiết.", "/images/tiemphong.jpg", "Tiêm phòng & khám tổng quát", 150000m }
                            });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
