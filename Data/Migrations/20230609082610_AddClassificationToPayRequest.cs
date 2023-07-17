using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JV_Imprest_Payment.Data.Migrations
{
    public partial class AddClassificationToPayRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvidenceOfPayment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Assignedto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requester = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequesterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JVA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JVADate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Accountant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountantDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayRequest_AfeStructure_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "AfeStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayRequest_CategoryId",
                table: "PayRequest",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayRequest");
        }
    }
}
