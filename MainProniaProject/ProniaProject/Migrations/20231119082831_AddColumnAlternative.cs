using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProniaProject.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnAlternative : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alternative",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alternative",
                table: "ProductImages");
        }
    }
}
