﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAPIAutores.Migrations
{
    public partial class UserComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Comentarios",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UserId",
                table: "Comentarios",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_AspNetUsers_UserId",
                table: "Comentarios",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_AspNetUsers_UserId",
                table: "Comentarios");

            migrationBuilder.DropIndex(
                name: "IX_Comentarios_UserId",
                table: "Comentarios");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comentarios");
        }
    }
}
