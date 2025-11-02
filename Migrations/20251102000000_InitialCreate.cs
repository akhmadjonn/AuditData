using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuditData.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Spreadsheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Broker = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LoadId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PUDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Origin = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DELDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Destination = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Mileage = table.Column<int>(type: "integer", nullable: true),
                    PerMile = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Rate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Dispatcher = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InvoicedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    DispatchNotes = table.Column<string>(type: "text", nullable: true),
                    AccountingNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spreadsheets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoadNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CheckNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DebtorName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    FeeDays = table.Column<int>(type: "integer", nullable: true),
                    InvoiceAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ActivityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CheckAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    SheetName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_InvoiceNumber",
                table: "Payments",
                column: "InvoiceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_LoadNumber",
                table: "Payments",
                column: "LoadNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_SheetName",
                table: "Payments",
                column: "SheetName");

            migrationBuilder.CreateIndex(
                name: "IX_Spreadsheets_LoadId",
                table: "Spreadsheets",
                column: "LoadId");

            migrationBuilder.CreateIndex(
                name: "IX_Spreadsheets_Status",
                table: "Spreadsheets",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Spreadsheets");
        }
    }
}
