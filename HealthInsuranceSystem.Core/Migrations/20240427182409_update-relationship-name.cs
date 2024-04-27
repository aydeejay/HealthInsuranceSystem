using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthInsuranceSystem.Core.Migrations
{
    /// <inheritdoc />
    public partial class updaterelationshipname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Users_PolicyHolderId",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "PolicyHolderId",
                table: "Claims",
                newName: "PolicyHolderUserId");

            migrationBuilder.RenameColumn(
                name: "ClaimId",
                table: "Claims",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Claims_PolicyHolderId",
                table: "Claims",
                newName: "IX_Claims_PolicyHolderUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Users_PolicyHolderUserId",
                table: "Claims",
                column: "PolicyHolderUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Users_PolicyHolderUserId",
                table: "Claims");

            migrationBuilder.RenameColumn(
                name: "PolicyHolderUserId",
                table: "Claims",
                newName: "PolicyHolderId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Claims",
                newName: "ClaimId");

            migrationBuilder.RenameIndex(
                name: "IX_Claims_PolicyHolderUserId",
                table: "Claims",
                newName: "IX_Claims_PolicyHolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Users_PolicyHolderId",
                table: "Claims",
                column: "PolicyHolderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
