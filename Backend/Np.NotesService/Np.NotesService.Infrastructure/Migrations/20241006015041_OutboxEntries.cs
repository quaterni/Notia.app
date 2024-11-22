using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Np.NotesService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OutboxEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    refresh_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    event_name = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_entries", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_entries");
        }
    }
}
