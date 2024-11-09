using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Np.UsersService.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddConcurrentToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "users",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "users");
        }
    }
}
