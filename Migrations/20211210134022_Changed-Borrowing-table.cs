using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryMs.Migrations
{
    public partial class ChangedBorrowingtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RetrunDate",
                table: "Borrowing",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RetrunDate",
                table: "Borrowing");
        }
    }
}
