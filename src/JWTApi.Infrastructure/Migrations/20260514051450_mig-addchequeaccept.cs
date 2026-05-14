using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migaddchequeaccept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChequeDateResult",
                table: "Cheques",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChequeDate_PersionResult",
                table: "Cheques",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionRowResult",
                table: "Cheques",
                type: "nvarchar(350)",
                maxLength: 350,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChequeDateResult",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "ChequeDate_PersionResult",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "DescriptionRowResult",
                table: "Cheques");
        }
    }
}
