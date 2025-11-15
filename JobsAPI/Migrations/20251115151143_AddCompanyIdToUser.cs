using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobsAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "companyid",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_companyid",
                table: "users",
                column: "companyid");

            migrationBuilder.AddForeignKey(
                name: "FK_users_companies_companyid",
                table: "users",
                column: "companyid",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_companies_companyid",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_companyid",
                table: "users");

            migrationBuilder.DropColumn(
                name: "companyid",
                table: "users");
        }
    }
}
