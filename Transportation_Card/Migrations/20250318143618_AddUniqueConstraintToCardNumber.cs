using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transportation_Card.Migrations
{

    public partial class AddUniqueConstraintToCardNumber : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_CardNumber",
                table: "Users",
                column: "CardNumber",
                unique: true);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_CardNumber",
                table: "Users");
        }
    }
}
