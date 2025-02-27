using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskSphere.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "TaskUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "TaskUsers");
        }
    }
}
