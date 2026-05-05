using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migaddfinan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Financial_Financial_FinancialId",
                table: "Financial");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialCompanies_Financial_FinancialId",
                table: "FinancialCompanies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Financial",
                table: "Financial");

            migrationBuilder.RenameTable(
                name: "Financial",
                newName: "Financials");

            migrationBuilder.RenameIndex(
                name: "IX_Financial_FinancialId",
                table: "Financials",
                newName: "IX_Financials_FinancialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Financials",
                table: "Financials",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialCompanies_Financials_FinancialId",
                table: "FinancialCompanies",
                column: "FinancialId",
                principalTable: "Financials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Financials_Financials_FinancialId",
                table: "Financials",
                column: "FinancialId",
                principalTable: "Financials",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialCompanies_Financials_FinancialId",
                table: "FinancialCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_Financials_Financials_FinancialId",
                table: "Financials");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Financials",
                table: "Financials");

            migrationBuilder.RenameTable(
                name: "Financials",
                newName: "Financial");

            migrationBuilder.RenameIndex(
                name: "IX_Financials_FinancialId",
                table: "Financial",
                newName: "IX_Financial_FinancialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Financial",
                table: "Financial",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Financial_Financial_FinancialId",
                table: "Financial",
                column: "FinancialId",
                principalTable: "Financial",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialCompanies_Financial_FinancialId",
                table: "FinancialCompanies",
                column: "FinancialId",
                principalTable: "Financial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
