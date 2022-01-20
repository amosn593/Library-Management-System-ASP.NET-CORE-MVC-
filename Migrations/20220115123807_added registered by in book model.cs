using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryMs.Migrations
{
    public partial class addedregisteredbyinbookmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegisteredBy",
                table: "Book",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredBy",
                table: "Book");
        }
    }
}
