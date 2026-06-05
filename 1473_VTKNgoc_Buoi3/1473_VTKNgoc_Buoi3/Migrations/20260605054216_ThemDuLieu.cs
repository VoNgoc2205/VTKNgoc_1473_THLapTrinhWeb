using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _1473_VTKNgoc_Buoi3.Migrations
{
    /// <inheritdoc />
    public partial class ThemDuLieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Thức ăn thú cưng" },
                    { 2, "Phụ kiện" },
                    { 3, "Chăm sóc sức khỏe" },
                    { 4, "Dịch vụ chăm sóc" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Hạt giàu protein, bổ sung vitamin giúp mèo khỏe mạnh và lông mượt.", "/images/Hat.jpg", "Hạt dinh dưỡng cho mèo", 185000m },
                    { 2, 2, "Gối ngủ êm ái, giúp thú cưng nghỉ ngơi thoải mái trong mọi góc nhà.", "/images/GoiNgu.jpg", "Gối ngủ mềm cho thú cưng", 249000m },
                    { 3, 3, "Không gian leo trèo và nghỉ ngơi gọn đẹp, phù hợp cho mèo năng động.", "/images/Cat.jpg", "Nhà cây cho mèo", 390000m },
                    { 4, 4, "Dịch vụ tắm, sấy và cắt tỉa lông giúp thú cưng sạch sẽ, thơm tho và thoải mái.", "/images/pet-banner.jpg", "Spa & Grooming cao cấp", 200000m },
                    { 5, 4, "Không gian lưu trú sạch sẽ, có khu vui chơi và nhân viên theo dõi thú cưng trong ngày.", "/images/GoiNgu.jpg", "Khách sạn cho thú cưng", 350000m },
                    { 6, 4, "Gói kiểm tra sức khỏe định kỳ và tư vấn tiêm phòng cần thiết cho chó mèo.", "/images/Cat.jpg", "Tiêm phòng & khám tổng quát", 150000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
