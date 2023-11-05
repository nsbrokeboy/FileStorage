using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileStorage.Infrastructure.Data.Migrations
{
    public partial class EnableNullableInTempLink2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryLinks_FileGroups_FileGroupId",
                table: "TemporaryLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryLinks_Files_FileId",
                table: "TemporaryLinks");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileId",
                table: "TemporaryLinks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileGroupId",
                table: "TemporaryLinks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryLinks_FileGroups_FileGroupId",
                table: "TemporaryLinks",
                column: "FileGroupId",
                principalTable: "FileGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryLinks_Files_FileId",
                table: "TemporaryLinks",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryLinks_FileGroups_FileGroupId",
                table: "TemporaryLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TemporaryLinks_Files_FileId",
                table: "TemporaryLinks");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileId",
                table: "TemporaryLinks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FileGroupId",
                table: "TemporaryLinks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryLinks_FileGroups_FileGroupId",
                table: "TemporaryLinks",
                column: "FileGroupId",
                principalTable: "FileGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TemporaryLinks_Files_FileId",
                table: "TemporaryLinks",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
