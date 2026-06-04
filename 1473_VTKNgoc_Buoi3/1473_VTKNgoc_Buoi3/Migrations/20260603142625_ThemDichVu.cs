using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                values: new object[] { 4, "D?ch v? cham s�c" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 4, 4, "D?ch v? t?m, s?y v� c?t t?a l�ng gi�p th� cung s?ch s?, thom tho v� tho?i m�i.", "/images/tam.jpg", "Spa & Grooming cao c?p", 200000m },
                    { 5, 4, "Kh�ng gian luu tr� s?ch s?, c� khu vui choi v� nh�n vi�n theo d�i th� cung trong ng�y.", "/images/kstc.jpg", "Kh�ch s?n cho th� cung", 350000m },
                    { 6, 4, "G�i ki?m tra s?c kh?e d?nh k? v� tu v?n ti�m ph�ng c?n thi?t cho ch� m�o.", "/images/tiemphong.jpg", "Ti�m ph�ng & kh�m t?ng qu�t", 150000m }
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

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
