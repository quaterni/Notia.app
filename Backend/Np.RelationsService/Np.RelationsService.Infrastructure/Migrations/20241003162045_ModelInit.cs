using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Np.RelationsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "relations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    outgoing_id = table.Column<Guid>(type: "uuid", nullable: false),
                    incoming_id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relations", x => x.id);
                    table.ForeignKey(
                        name: "fk_relations_notes_incoming_id",
                        column: x => x.incoming_id,
                        principalTable: "notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_relations_notes_outgoing_id",
                        column: x => x.outgoing_id,
                        principalTable: "notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "root_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_root_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_root_entries_notes_id",
                        column: x => x.id,
                        principalTable: "notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_relations_incoming_id",
                table: "relations",
                column: "incoming_id");

            migrationBuilder.CreateIndex(
                name: "ix_relations_outgoing_id",
                table: "relations",
                column: "outgoing_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "relations");

            migrationBuilder.DropTable(
                name: "root_entries");

            migrationBuilder.DropTable(
                name: "notes");
        }
    }
}
