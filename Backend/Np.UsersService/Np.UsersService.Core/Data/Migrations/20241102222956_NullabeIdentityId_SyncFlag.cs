using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Np.UsersService.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class NullabeIdentityId_SyncFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_users_IdentityId",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityId",
                table: "users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "IsSyncrhonizedWithIdentity",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_users_IdentityId",
                table: "users",
                column: "IdentityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_IdentityId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IsSyncrhonizedWithIdentity",
                table: "users");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityId",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_users_IdentityId",
                table: "users",
                column: "IdentityId");
        }
    }
}
