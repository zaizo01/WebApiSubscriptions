using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPIAutores.Migrations
{
    public partial class PoorPay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PoorPay",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PoorPay",
                table: "AspNetUsers");
        }
    }
}
