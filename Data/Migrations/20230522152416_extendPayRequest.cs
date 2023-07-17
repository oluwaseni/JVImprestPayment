using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JV_Imprest_Payment.Data.Migrations
{
    public partial class extendPayRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Accountant",
                table: "PayRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountantDate",
                table: "PayRequest",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "JVA",
                table: "PayRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "JVADate",
                table: "PayRequest",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Requester",
                table: "PayRequest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequesterDate",
                table: "PayRequest",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accountant",
                table: "PayRequest");

            migrationBuilder.DropColumn(
                name: "AccountantDate",
                table: "PayRequest");

            migrationBuilder.DropColumn(
                name: "JVA",
                table: "PayRequest");

            migrationBuilder.DropColumn(
                name: "JVADate",
                table: "PayRequest");

            migrationBuilder.DropColumn(
                name: "Requester",
                table: "PayRequest");

            migrationBuilder.DropColumn(
                name: "RequesterDate",
                table: "PayRequest");
        }
    }
}
