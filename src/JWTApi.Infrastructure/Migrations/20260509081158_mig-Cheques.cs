using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migCheques : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountSideId",
                table: "FinancialOperations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AmountCash",
                table: "FinancialOperations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AmountCheque",
                table: "FinancialOperations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinancialOperationsId",
                table: "Cheques",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialOperations_AccountSideId",
                table: "FinancialOperations",
                column: "AccountSideId");

            migrationBuilder.CreateIndex(
                name: "IX_Cheques_FinancialOperationsId",
                table: "Cheques",
                column: "FinancialOperationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cheques_FinancialOperations_FinancialOperationsId",
                table: "Cheques",
                column: "FinancialOperationsId",
                principalTable: "FinancialOperations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialOperations_AccountSides_AccountSideId",
                table: "FinancialOperations",
                column: "AccountSideId",
                principalTable: "AccountSides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cheques_FinancialOperations_FinancialOperationsId",
                table: "Cheques");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialOperations_AccountSides_AccountSideId",
                table: "FinancialOperations");

            migrationBuilder.DropIndex(
                name: "IX_FinancialOperations_AccountSideId",
                table: "FinancialOperations");

            migrationBuilder.DropIndex(
                name: "IX_Cheques_FinancialOperationsId",
                table: "Cheques");

            migrationBuilder.DropColumn(
                name: "AccountSideId",
                table: "FinancialOperations");

            migrationBuilder.DropColumn(
                name: "AmountCash",
                table: "FinancialOperations");

            migrationBuilder.DropColumn(
                name: "AmountCheque",
                table: "FinancialOperations");

            migrationBuilder.DropColumn(
                name: "FinancialOperationsId",
                table: "Cheques");
        }
    }
}
