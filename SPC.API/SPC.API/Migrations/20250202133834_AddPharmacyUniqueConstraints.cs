using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPC.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Pharmacies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_Email",
                table: "Pharmacies",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pharmacies_RegistrationNumber",
                table: "Pharmacies",
                column: "RegistrationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_Email",
                table: "Pharmacies");

            migrationBuilder.DropIndex(
                name: "IX_Pharmacies_RegistrationNumber",
                table: "Pharmacies");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Pharmacies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
