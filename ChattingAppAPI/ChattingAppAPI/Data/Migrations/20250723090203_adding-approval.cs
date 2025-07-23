using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChattingAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class addingapproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsApproval",
                table: "Photos",
                newName: "IsApproved");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "Photos",
                newName: "IsApproval");
        }
    }
}
