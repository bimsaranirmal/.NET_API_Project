using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPC.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatev9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierDrugs_Suppliers_SupplierId",
                table: "SupplierDrugs");

            migrationBuilder.DropIndex(
                name: "IX_SupplierDrugs_SupplierId",
                table: "SupplierDrugs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SupplierDrugs_SupplierId",
                table: "SupplierDrugs",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierDrugs_Suppliers_SupplierId",
                table: "SupplierDrugs",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
