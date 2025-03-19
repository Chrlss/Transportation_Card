using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transportation_Card.Migrations
{

    public partial class AddSeniorCitizenAndPwdFields : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PwdId",
                table: "Users",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "SeniorCitizenCard",
                table: "Users",
                type: "longtext",
                nullable: false);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PwdId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SeniorCitizenCard",
                table: "Users");
        }
    }
}
