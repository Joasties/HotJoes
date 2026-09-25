using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotJoes.Infrastructure.Community.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommunityParticipationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "community");

            migrationBuilder.CreateTable(
                name: "community_participations",
                schema: "community",
                columns: table => new
                {
                    community_participation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contact_preference = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    joined_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_community_participations", x => x.community_participation_id);
                    table.CheckConstraint("ck_community_participations_contact_preference", "\"contact_preference\" IN ('email', 'sms', 'whatsApp')");
                });

            migrationBuilder.CreateTable(
                name: "community_participation_outbox",
                schema: "community",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    community_participation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_version = table.Column<int>(type: "integer", nullable: false),
                    serialized_event = table.Column<byte[]>(type: "bytea", nullable: false),
                    trace_parent = table.Column<string>(type: "character varying(55)", maxLength: 55, nullable: true),
                    trace_state = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    attempt_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    next_attempt_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    claimed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    claim_expires_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_attempt_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_failure_category = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    is_stalled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    published_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_community_participation_outbox", x => x.event_id);
                    table.CheckConstraint("ck_community_outbox_attempt_count", "\"attempt_count\" >= 0");
                    table.CheckConstraint("ck_community_outbox_claim", "(\"claimed_by\" IS NULL AND \"claim_expires_at_utc\" IS NULL) OR (\"claimed_by\" IS NOT NULL AND \"claim_expires_at_utc\" IS NOT NULL)");
                    table.CheckConstraint("ck_community_outbox_event_version", "\"event_version\" > 0");
                    table.CheckConstraint("ck_community_outbox_serialized_event", "octet_length(\"serialized_event\") > 0");
                    table.CheckConstraint("ck_community_outbox_stalled", "NOT \"is_stalled\" OR \"next_attempt_at_utc\" IS NULL");
                    table.ForeignKey(
                        name: "FK_community_participation_outbox_community_participations_com~",
                        column: x => x.community_participation_id,
                        principalSchema: "community",
                        principalTable: "community_participations",
                        principalColumn: "community_participation_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_community_participation_outbox_eligible",
                schema: "community",
                table: "community_participation_outbox",
                columns: new[] { "next_attempt_at_utc", "claim_expires_at_utc", "event_id" },
                filter: "published_at_utc IS NULL AND is_stalled = FALSE");

            migrationBuilder.CreateIndex(
                name: "ix_community_participation_outbox_unpublished",
                schema: "community",
                table: "community_participation_outbox",
                column: "event_id",
                filter: "published_at_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "uq_community_participation_outbox_participation_id",
                schema: "community",
                table: "community_participation_outbox",
                column: "community_participation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_community_participations_vendor_id",
                schema: "community",
                table: "community_participations",
                column: "vendor_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "community_participation_outbox",
                schema: "community");

            migrationBuilder.DropTable(
                name: "community_participations",
                schema: "community");
        }
    }
}
