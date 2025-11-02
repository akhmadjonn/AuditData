using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditData.Migrations
{
    /// <inheritdoc />
    public partial class ConvertDatesToStrings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Spreadsheets table - convert DateTime columns to string
            migrationBuilder.AlterColumn<string>(
                name: "PUDate",
                table: "Spreadsheets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DELDate",
                table: "Spreadsheets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAt",
                table: "Spreadsheets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            // Payments table - convert DateTime columns to string
            migrationBuilder.AlterColumn<string>(
                name: "PurchaseDate",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentDate",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedAt",
                table: "Payments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Spreadsheets table
            migrationBuilder.AlterColumn<DateTime>(
                name: "PUDate",
                table: "Spreadsheets",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DELDate",
                table: "Spreadsheets",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Spreadsheets",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            // Revert Payments table
            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Payments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
