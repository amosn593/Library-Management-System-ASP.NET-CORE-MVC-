using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryMs.Migrations
{
    public partial class AddedIssuedfieldonborrowing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Issued",
                table: "Borrowing",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Yes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Issued",
                table: "Borrowing");
        }
    }
}
