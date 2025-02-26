using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.SqlServerMigrations
{
    /// <inheritdoc />
    public partial class UpdateHospital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hospitals_Hospitals_HospitalOid",
                table: "Hospitals");

            migrationBuilder.DropIndex(
                name: "IX_Hospitals_HospitalOid",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "HospitalOid",
                table: "Hospitals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalOid",
                table: "Hospitals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hospitals_HospitalOid",
                table: "Hospitals",
                column: "HospitalOid");

            migrationBuilder.AddForeignKey(
                name: "FK_Hospitals_Hospitals_HospitalOid",
                table: "Hospitals",
                column: "HospitalOid",
                principalTable: "Hospitals",
                principalColumn: "Oid");
        }
    }
}
