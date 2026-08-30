using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotJoes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReliableOutboxRelayState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "attempt_count",
                table: "vendor_registration_outbox",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "claim_expires_at_utc",
                table: "vendor_registration_outbox",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "claimed_by",
                table: "vendor_registration_outbox",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_stalled",
                table: "vendor_registration_outbox",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_attempt_at_utc",
                table: "vendor_registration_outbox",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_failure_category",
                table: "vendor_registration_outbox",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "next_attempt_at_utc",
                table: "vendor_registration_outbox",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_vendor_registration_outbox_eligible",
                table: "vendor_registration_outbox",
                columns: new[] { "next_attempt_at_utc", "claim_expires_at_utc", "event_id" },
                filter: "published_at_utc IS NULL AND is_stalled = FALSE");

            migrationBuilder.AddCheckConstraint(
                name: "ck_vendor_registration_outbox_attempt_count",
                table: "vendor_registration_outbox",
                sql: "\"attempt_count\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_vendor_registration_outbox_claim",
                table: "vendor_registration_outbox",
                sql: "(\"claimed_by\" IS NULL AND \"claim_expires_at_utc\" IS NULL) OR (\"claimed_by\" IS NOT NULL AND \"claim_expires_at_utc\" IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "ck_vendor_registration_outbox_stalled",
                table: "vendor_registration_outbox",
                sql: "NOT \"is_stalled\" OR \"next_attempt_at_utc\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_vendor_registration_outbox_eligible",
                table: "vendor_registration_outbox");

            migrationBuilder.DropCheckConstraint(
                name: "ck_vendor_registration_outbox_attempt_count",
                table: "vendor_registration_outbox");

            migrationBuilder.DropCheckConstraint(
                name: "ck_vendor_registration_outbox_claim",
                table: "vendor_registration_outbox");

            migrationBuilder.DropCheckConstraint(
                name: "ck_vendor_registration_outbox_stalled",
                table: "vendor_registration_outbox");

            migrationBuilder.DropColumn(
                name: "attempt_count",
                table: "vendor_registration_outbox");

            migrationBuilder.DropColumn(
                name: "claim_expires_at_utc",
                table: "vendor_registration_outbox");

            migrationBuilder.DropColumn(
                name: "claimed_by",
                table: "vendor_registration_outbox");

            migrationBuilder.DropColumn(
                name: "is_stalled",
                table: "vendor_registration_outbox");

            migrationBuilder.DropColumn(
                name: "last_attempt_at_utc",
                table: "vendor_registration_outbox");

            migrationBuilder.DropColumn(
                name: "last_failure_category",
                table: "vendor_registration_outbox");

            migrationBuilder.DropColumn(
                name: "next_attempt_at_utc",
                table: "vendor_registration_outbox");
        }
    }
}
