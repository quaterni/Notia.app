using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Np.UsersService.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_outbox_entries",
                table: "outbox_entries");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "users",
                newName: "username");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsSyncrhonizedWithIdentity",
                table: "users",
                newName: "is_syncrhonized_with_identity");

            migrationBuilder.RenameColumn(
                name: "IdentityId",
                table: "users",
                newName: "identity_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_Username",
                table: "users",
                newName: "ix_users_username");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "ix_users_email");

            migrationBuilder.RenameIndex(
                name: "IX_users_IdentityId",
                table: "users",
                newName: "ix_users_identity_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "outbox_entries",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "outbox_entries",
                newName: "data");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "outbox_entries",
                newName: "created");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "outbox_entries",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RefreshTime",
                table: "outbox_entries",
                newName: "refresh_time");

            migrationBuilder.RenameIndex(
                name: "IX_outbox_entries_RefreshTime",
                table: "outbox_entries",
                newName: "ix_outbox_entries_refresh_time");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_outbox_entries",
                table: "outbox_entries",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_outbox_entries",
                table: "outbox_entries");

            migrationBuilder.RenameColumn(
                name: "username",
                table: "users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_syncrhonized_with_identity",
                table: "users",
                newName: "IsSyncrhonizedWithIdentity");

            migrationBuilder.RenameColumn(
                name: "identity_id",
                table: "users",
                newName: "IdentityId");

            migrationBuilder.RenameIndex(
                name: "ix_users_username",
                table: "users",
                newName: "IX_users_Username");

            migrationBuilder.RenameIndex(
                name: "ix_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameIndex(
                name: "ix_users_identity_id",
                table: "users",
                newName: "IX_users_IdentityId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "outbox_entries",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "data",
                table: "outbox_entries",
                newName: "Data");

            migrationBuilder.RenameColumn(
                name: "created",
                table: "outbox_entries",
                newName: "Created");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "outbox_entries",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "refresh_time",
                table: "outbox_entries",
                newName: "RefreshTime");

            migrationBuilder.RenameIndex(
                name: "ix_outbox_entries_refresh_time",
                table: "outbox_entries",
                newName: "IX_outbox_entries_RefreshTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_outbox_entries",
                table: "outbox_entries",
                column: "Id");
        }
    }
}
