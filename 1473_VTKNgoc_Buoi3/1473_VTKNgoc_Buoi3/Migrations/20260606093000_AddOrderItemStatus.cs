using _1473_VTKNgoc_Buoi3.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1473_VTKNgoc_Buoi3.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260606093000_AddOrderItemStatus")]
    public partial class AddOrderItemStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemStatus",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Chờ xử lý");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemStatus",
                table: "OrderItems");
        }
    }
}
