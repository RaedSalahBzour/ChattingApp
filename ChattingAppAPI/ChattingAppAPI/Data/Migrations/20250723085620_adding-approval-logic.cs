using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChattingAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class addingapprovallogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproval",
                table: "Photos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproval",
                table: "Photos");
        }
    }
}
