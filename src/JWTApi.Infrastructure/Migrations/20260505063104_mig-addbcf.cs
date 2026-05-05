using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migaddbcf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankCompany_Banks_BankId",
                table: "BankCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_BankCompany_Companies_CompanyId",
                table: "BankCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialCompany_Companies_CompanyId",
                table: "FinancialCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialCompany_Financial_FinancialId",
                table: "FinancialCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinancialCompany",
                table: "FinancialCompany");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankCompany",
                table: "BankCompany");

            migrationBuilder.RenameTable(
                name: "FinancialCompany",
                newName: "FinancialCompanies");

            migrationBuilder.RenameTable(
                name: "BankCompany",
                newName: "BankCompanies");

            migrationBuilder.RenameIndex(
                name: "IX_FinancialCompany_CompanyId",
                table: "FinancialCompanies",
                newName: "IX_FinancialCompanies_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_BankCompany_CompanyId",
                table: "BankCompanies",
                newName: "IX_BankCompanies_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinancialCompanies",
                table: "FinancialCompanies",
                columns: new[] { "FinancialId", "CompanyId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankCompanies",
                table: "BankCompanies",
                columns: new[] { "BankId", "CompanyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BankCompanies_Banks_BankId",
                table: "BankCompanies",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankCompanies_Companies_CompanyId",
                table: "BankCompanies",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialCompanies_Companies_CompanyId",
                table: "FinancialCompanies",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialCompanies_Financial_FinancialId",
                table: "FinancialCompanies",
                column: "FinancialId",
                principalTable: "Financial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankCompanies_Banks_BankId",
                table: "BankCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_BankCompanies_Companies_CompanyId",
                table: "BankCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialCompanies_Companies_CompanyId",
                table: "FinancialCompanies");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialCompanies_Financial_FinancialId",
                table: "FinancialCompanies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinancialCompanies",
                table: "FinancialCompanies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankCompanies",
                table: "BankCompanies");

            migrationBuilder.RenameTable(
                name: "FinancialCompanies",
                newName: "FinancialCompany");

            migrationBuilder.RenameTable(
                name: "BankCompanies",
                newName: "BankCompany");

            migrationBuilder.RenameIndex(
                name: "IX_FinancialCompanies_CompanyId",
                table: "FinancialCompany",
                newName: "IX_FinancialCompany_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_BankCompanies_CompanyId",
                table: "BankCompany",
                newName: "IX_BankCompany_CompanyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinancialCompany",
                table: "FinancialCompany",
                columns: new[] { "FinancialId", "CompanyId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankCompany",
                table: "BankCompany",
                columns: new[] { "BankId", "CompanyId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BankCompany_Banks_BankId",
                table: "BankCompany",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BankCompany_Companies_CompanyId",
                table: "BankCompany",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialCompany_Companies_CompanyId",
                table: "FinancialCompany",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialCompany_Financial_FinancialId",
                table: "FinancialCompany",
                column: "FinancialId",
                principalTable: "Financial",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
