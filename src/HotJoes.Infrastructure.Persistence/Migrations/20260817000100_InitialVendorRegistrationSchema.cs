using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotJoes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialVendorRegistrationSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vendor_registrations",
                columns: table => new
                {
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_state = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    trading_preference = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    registered_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    legal_operator_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    legal_operator_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    normalized_legal_operator_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    trading_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    normalized_trading_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    company_registration_number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    contact_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contact_email = table.Column<string>(type: "text", nullable: false),
                    contact_telephone = table.Column<string>(type: "text", nullable: false),
                    canonical_address_id = table.Column<string>(type: "text", nullable: false),
                    recipient_or_organisation_name = table.Column<string>(type: "text", nullable: true),
                    address_line_1 = table.Column<string>(type: "text", nullable: false),
                    address_line_2 = table.Column<string>(type: "text", nullable: true),
                    address_line_3 = table.Column<string>(type: "text", nullable: true),
                    post_town = table.Column<string>(type: "text", nullable: false),
                    postcode = table.Column<string>(type: "text", nullable: false),
                    county = table.Column<string>(type: "text", nullable: true),
                    food_registration_authority = table.Column<string>(type: "text", nullable: false),
                    primary_trading_authority = table.Column<string>(type: "text", nullable: true),
                    trading_location = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    opening_hours_start = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    opening_hours_end = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    service_includes_hot_food = table.Column<bool>(type: "boolean", nullable: false),
                    alcohol_service = table.Column<bool>(type: "boolean", nullable: false),
                    website = table.Column<string>(type: "text", nullable: true),
                    business_description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor_registrations", x => x.vendor_id);
                    table.CheckConstraint("ck_vendor_registrations_company_registration_number", "((\"legal_operator_type\" IN ('limitedCompany', 'limitedLiabilityPartnership', 'charitableIncorporatedOrganisation') AND \"company_registration_number\" IS NOT NULL) OR (\"legal_operator_type\" NOT IN ('limitedCompany', 'limitedLiabilityPartnership', 'charitableIncorporatedOrganisation') AND \"company_registration_number\" IS NULL))");
                    table.CheckConstraint("ck_vendor_registrations_legal_operator_type", "\"legal_operator_type\" IN ('soleTrader', 'generalPartnership', 'limitedCompany', 'limitedLiabilityPartnership', 'charitableCommunityGroup', 'charitableIncorporatedOrganisation')");
                    table.CheckConstraint("ck_vendor_registrations_normalized_names", "length(\"normalized_trading_name\") > 0 AND \"normalized_trading_name\" = lower(btrim(\"normalized_trading_name\")) AND length(\"normalized_legal_operator_name\") > 0 AND \"normalized_legal_operator_name\" = lower(btrim(\"normalized_legal_operator_name\"))");
                    table.CheckConstraint("ck_vendor_registrations_primary_trading_authority", "((\"trading_location\" = 'stall' AND \"primary_trading_authority\" IS NOT NULL) OR (\"trading_location\" <> 'stall' AND \"primary_trading_authority\" IS NULL))");
                    table.CheckConstraint("ck_vendor_registrations_trading_location", "\"trading_location\" IN ('restaurant', 'stall', 'kitchen')");
                    table.CheckConstraint("ck_vendor_registrations_trading_preference", "\"trading_preference\" IN ('offline', 'online')");
                    table.CheckConstraint("ck_vendor_registrations_vendor_state", "\"vendor_state\" IN ('pendingActivation', 'activated', 'suspended', 'deactivated')");
                });

            migrationBuilder.CreateTable(
                name: "vendor_registration_outbox",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_version = table.Column<int>(type: "integer", nullable: false),
                    serialized_event = table.Column<byte[]>(type: "bytea", nullable: false),
                    published_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor_registration_outbox", x => x.event_id);
                    table.CheckConstraint("ck_vendor_registration_outbox_event_version", "\"event_version\" > 0");
                    table.CheckConstraint("ck_vendor_registration_outbox_serialized_event", "octet_length(\"serialized_event\") > 0");
                    table.ForeignKey(
                        name: "FK_vendor_registration_outbox_vendor_registrations_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendor_registrations",
                        principalColumn: "vendor_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vendor_registration_outcomes",
                columns: table => new
                {
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fingerprint_version = table.Column<short>(type: "smallint", nullable: false),
                    semantic_fingerprint_sha256 = table.Column<byte[]>(type: "bytea", nullable: false),
                    result_vendor_state = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor_registration_outcomes", x => x.vendor_id);
                    table.CheckConstraint("ck_vendor_registration_outcomes_fingerprint_sha256", "octet_length(\"semantic_fingerprint_sha256\") = 32");
                    table.CheckConstraint("ck_vendor_registration_outcomes_fingerprint_version", "\"fingerprint_version\" > 0");
                    table.CheckConstraint("ck_vendor_registration_outcomes_result_vendor_state", "\"result_vendor_state\" = 'pendingActivation'");
                    table.ForeignKey(
                        name: "FK_vendor_registration_outcomes_vendor_registrations_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendor_registrations",
                        principalColumn: "vendor_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vendor_registration_outbox_unpublished",
                table: "vendor_registration_outbox",
                column: "event_id",
                filter: "published_at_utc IS NULL");

            migrationBuilder.CreateIndex(
                name: "uq_vendor_registration_outbox_vendor_id",
                table: "vendor_registration_outbox",
                column: "vendor_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "uq_vendor_registrations_identity",
                table: "vendor_registrations",
                columns: new[] { "normalized_trading_name", "normalized_legal_operator_name", "canonical_address_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vendor_registration_outbox");

            migrationBuilder.DropTable(
                name: "vendor_registration_outcomes");

            migrationBuilder.DropTable(
                name: "vendor_registrations");
        }
    }
}
