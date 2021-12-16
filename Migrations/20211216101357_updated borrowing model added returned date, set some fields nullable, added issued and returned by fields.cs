using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryMs.Migrations
{
    public partial class updatedborrowingmodeladdedreturneddatesetsomefieldsnullableaddedissuedandreturnedbyfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnedDate",
                table: "Borrowing",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "IssuedBy",
                table: "Borrowing",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnedBy",
                table: "Borrowing",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuedBy",
                table: "Borrowing");

            migrationBuilder.DropColumn(
                name: "ReturnedBy",
                table: "Borrowing");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnedDate",
                table: "Borrowing",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
