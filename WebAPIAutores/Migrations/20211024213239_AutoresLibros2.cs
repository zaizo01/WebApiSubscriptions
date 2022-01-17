using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPIAutores.Migrations
{
    public partial class AutoresLibros2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoresLibros_Libros_LibroId",
                table: "AutoresLibros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AutoresLibros",
                table: "AutoresLibros");

            migrationBuilder.DropColumn(
                name: "LibrosId",
                table: "AutoresLibros");

            migrationBuilder.AlterColumn<int>(
                name: "LibroId",
                table: "AutoresLibros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutoresLibros",
                table: "AutoresLibros",
                columns: new[] { "AutorId", "LibroId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AutoresLibros_Libros_LibroId",
                table: "AutoresLibros",
                column: "LibroId",
                principalTable: "Libros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AutoresLibros_Libros_LibroId",
                table: "AutoresLibros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AutoresLibros",
                table: "AutoresLibros");

            migrationBuilder.AlterColumn<int>(
                name: "LibroId",
                table: "AutoresLibros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LibrosId",
                table: "AutoresLibros",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutoresLibros",
                table: "AutoresLibros",
                columns: new[] { "AutorId", "LibrosId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AutoresLibros_Libros_LibroId",
                table: "AutoresLibros",
                column: "LibroId",
                principalTable: "Libros",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
