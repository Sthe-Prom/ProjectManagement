using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Migrations
{
    /// <inheritdoc />
    public partial class DB2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Project_ProjectId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_ProjectId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "ProjectStatus",
                table: "Project",
                newName: "SelectedAssignedUserIds");

            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                table: "Project",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProjectUpdateTime",
                table: "Project",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ActivityUpdateTime",
                table: "Activity",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Project_AccountID",
                table: "Project",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Account_AccountID",
                table: "Project",
                column: "AccountID",
                principalTable: "Account",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Account_AccountID",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_AccountID",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "AccountID",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProjectUpdateTime",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ActivityUpdateTime",
                table: "Activity");

            migrationBuilder.RenameColumn(
                name: "SelectedAssignedUserIds",
                table: "Project",
                newName: "ProjectStatus");

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Account",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_ProjectId",
                table: "Account",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Project_ProjectId",
                table: "Account",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");
        }
    }
}
