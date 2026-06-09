using _1473_VTKNgoc_Buoi3.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1473_VTKNgoc_Buoi3.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260606094500_AddPetDetailsToOrderItems")]
    public partial class AddPetDetailsToOrderItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PetAge",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetBreed",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetAge",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PetBreed",
                table: "OrderItems");
        }
    }
}
