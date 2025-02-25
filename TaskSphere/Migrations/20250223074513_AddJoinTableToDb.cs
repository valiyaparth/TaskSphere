using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskSphere.Migrations
{
    /// <inheritdoc />
    public partial class AddJoinTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_Projects_ProjectId",
                table: "ProjectMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMember_Users_UserId",
                table: "ProjectMember");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTeam_Projects_ProjectId",
                table: "ProjectTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTeam_Teams_TeamId",
                table: "ProjectTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUser_Tasks_TaskId",
                table: "TaskUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUser_Users_UserId",
                table: "TaskUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Users_UserId",
                table: "TeamMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMember",
                table: "TeamMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskUser",
                table: "TaskUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTeam",
                table: "ProjectTeam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMember",
                table: "ProjectMember");

            migrationBuilder.RenameTable(
                name: "TeamMember",
                newName: "TeamMembers");

            migrationBuilder.RenameTable(
                name: "TaskUser",
                newName: "TaskUsers");

            migrationBuilder.RenameTable(
                name: "ProjectTeam",
                newName: "ProjectTeams");

            migrationBuilder.RenameTable(
                name: "ProjectMember",
                newName: "ProjectMembers");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMember_UserId",
                table: "TeamMembers",
                newName: "IX_TeamMembers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskUser_UserId",
                table: "TaskUsers",
                newName: "IX_TaskUsers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTeam_TeamId",
                table: "ProjectTeams",
                newName: "IX_ProjectTeams_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMember_UserId",
                table: "ProjectMembers",
                newName: "IX_ProjectMembers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                columns: new[] { "TeamId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers",
                columns: new[] { "TaskId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTeams",
                table: "ProjectTeams",
                columns: new[] { "ProjectId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId",
                table: "ProjectMembers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMembers_Users_UserId",
                table: "ProjectMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTeams_Projects_ProjectId",
                table: "ProjectTeams",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTeams_Teams_TeamId",
                table: "ProjectTeams",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_Tasks_TaskId",
                table: "TaskUsers",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Users_UserId",
                table: "TeamMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Projects_ProjectId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectMembers_Users_UserId",
                table: "ProjectMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTeams_Projects_ProjectId",
                table: "ProjectTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTeams_Teams_TeamId",
                table: "ProjectTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_Tasks_TaskId",
                table: "TaskUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskUsers_Users_UserId",
                table: "TaskUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Users_UserId",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskUsers",
                table: "TaskUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectTeams",
                table: "ProjectTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectMembers",
                table: "ProjectMembers");

            migrationBuilder.RenameTable(
                name: "TeamMembers",
                newName: "TeamMember");

            migrationBuilder.RenameTable(
                name: "TaskUsers",
                newName: "TaskUser");

            migrationBuilder.RenameTable(
                name: "ProjectTeams",
                newName: "ProjectTeam");

            migrationBuilder.RenameTable(
                name: "ProjectMembers",
                newName: "ProjectMember");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMembers_UserId",
                table: "TeamMember",
                newName: "IX_TeamMember_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TaskUsers_UserId",
                table: "TaskUser",
                newName: "IX_TaskUser_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectTeams_TeamId",
                table: "ProjectTeam",
                newName: "IX_ProjectTeam_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectMembers_UserId",
                table: "ProjectMember",
                newName: "IX_ProjectMember_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMember",
                table: "TeamMember",
                columns: new[] { "TeamId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskUser",
                table: "TaskUser",
                columns: new[] { "TaskId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectTeam",
                table: "ProjectTeam",
                columns: new[] { "ProjectId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectMember",
                table: "ProjectMember",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_Projects_ProjectId",
                table: "ProjectMember",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectMember_Users_UserId",
                table: "ProjectMember",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTeam_Projects_ProjectId",
                table: "ProjectTeam",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTeam_Teams_TeamId",
                table: "ProjectTeam",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUser_Tasks_TaskId",
                table: "TaskUser",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskUser_Users_UserId",
                table: "TaskUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Users_UserId",
                table: "TeamMember",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
