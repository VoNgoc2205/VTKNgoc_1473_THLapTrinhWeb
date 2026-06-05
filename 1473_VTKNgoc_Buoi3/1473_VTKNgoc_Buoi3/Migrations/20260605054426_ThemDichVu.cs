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
     values: new object[] { 5, "Dịch vụ chăm sóc" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
        {
            7,
            5,
            "Dịch vụ tắm, sấy và cắt tỉa lông giúp thú cưng sạch sẽ, thơm tho và thoải mái.",
            "/images/tam.jpg",
            "Spa & Grooming cao cấp",
            200000m
        },
        {
            8,
            5,
            "Không gian lưu trú sạch sẽ, có khu vui chơi và nhân viên theo dõi thú cưng trong ngày.",
            "/images/kstc.jpg",
            "Khách sạn cho thú cưng",
            350000m
        },
        {
            9,
            5,
            "Gói kiểm tra sức khỏe định kỳ và tư vấn tiêm phòng cần thiết cho chó mèo.",
            "/images/tiemphong.jpg",
            "Tiêm phòng & khám tổng quát",
            150000m
        }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
