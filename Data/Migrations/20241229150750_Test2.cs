using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartFlow.Migrations
{
    /// <inheritdoc />
    public partial class Test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SavingsGoalId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SavingsGoalId",
                table: "Transactions",
                column: "SavingsGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_SavingsGoal_SavingsGoalId",
                table: "Transactions",
                column: "SavingsGoalId",
                principalTable: "SavingsGoal",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_SavingsGoal_SavingsGoalId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SavingsGoalId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SavingsGoalId",
                table: "Transactions");
        }
    }
}
