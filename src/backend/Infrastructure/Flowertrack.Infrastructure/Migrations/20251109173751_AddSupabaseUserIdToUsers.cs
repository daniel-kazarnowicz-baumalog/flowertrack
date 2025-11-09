using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flowertrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupabaseUserIdToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Tickets",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Tickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "ServiceUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ServiceUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SupabaseUserId",
                table: "ServiceUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "OrganizationUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "OrganizationUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvitationToken",
                table: "OrganizationUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "InvitationTokenExpiresAt",
                table: "OrganizationUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "OrganizationUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrganizationUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SupabaseUserId",
                table: "OrganizationUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Organizations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Organizations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Organizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Machines",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Machines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Machines",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceUsers_SupabaseUserId",
                table: "ServiceUsers",
                column: "SupabaseUserId",
                unique: true,
                filter: "\"SupabaseUserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUsers_SupabaseUserId",
                table: "OrganizationUsers",
                column: "SupabaseUserId",
                unique: true,
                filter: "\"SupabaseUserId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServiceUsers_SupabaseUserId",
                table: "ServiceUsers");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationUsers_SupabaseUserId",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ServiceUsers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ServiceUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceUsers");

            migrationBuilder.DropColumn(
                name: "SupabaseUserId",
                table: "ServiceUsers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "InvitationToken",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "InvitationTokenExpiresAt",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "SupabaseUserId",
                table: "OrganizationUsers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Machines");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Machines");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Machines");
        }
    }
}
