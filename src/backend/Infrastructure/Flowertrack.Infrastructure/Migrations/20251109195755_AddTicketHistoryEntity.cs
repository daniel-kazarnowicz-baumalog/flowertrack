using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowertrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketHistoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ticket_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticket_id = table.Column<Guid>(type: "uuid", nullable: false),
                    history_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    user_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    user_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    old_value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    new_value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    content = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    attachment_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    attachment_file_path = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    attachment_file_size = table.Column<long>(type: "bigint", nullable: true),
                    is_internal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_history", x => x.id);
                    table.ForeignKey(
                        name: "FK_ticket_history_Tickets_ticket_id",
                        column: x => x.ticket_id,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_history_created_at",
                table: "ticket_history",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_history_ticket_created",
                table: "ticket_history",
                columns: new[] { "ticket_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_history_ticket_id",
                table: "ticket_history",
                column: "ticket_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_history_user_id",
                table: "ticket_history",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_history");
        }
    }
}
