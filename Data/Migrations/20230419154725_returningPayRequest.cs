using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JV_Imprest_Payment.Data.Migrations
{
    public partial class returningPayRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AfeStructure_PayRequest_PayRequestId",
                table: "AfeStructure");

            migrationBuilder.DropIndex(
                name: "IX_AfeStructure_PayRequestId",
                table: "AfeStructure");

            migrationBuilder.DropColumn(
                name: "SelectedCategoryId",
                table: "PayRequest");

            migrationBuilder.DropColumn(
                name: "PayRequestId",
                table: "AfeStructure");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PayRequest",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "PayRequest");

            migrationBuilder.AddColumn<int>(
                name: "SelectedCategoryId",
                table: "PayRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayRequestId",
                table: "AfeStructure",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AfeStructure_PayRequestId",
                table: "AfeStructure",
                column: "PayRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AfeStructure_PayRequest_PayRequestId",
                table: "AfeStructure",
                column: "PayRequestId",
                principalTable: "PayRequest",
                principalColumn: "Id");
        }
    }
}
