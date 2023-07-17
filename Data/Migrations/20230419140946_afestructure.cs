using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JV_Imprest_Payment.Data.Migrations
{
    public partial class afestructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "AfeStructure",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AfeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Classification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AfeOwner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayRequestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AfeStructure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AfeStructure_PayRequest_PayRequestId",
                        column: x => x.PayRequestId,
                        principalTable: "PayRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AfeStructure_PayRequestId",
                table: "AfeStructure",
                column: "PayRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AfeStructure");

            migrationBuilder.DropColumn(
                name: "SelectedCategoryId",
                table: "PayRequest");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "PayRequest",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
