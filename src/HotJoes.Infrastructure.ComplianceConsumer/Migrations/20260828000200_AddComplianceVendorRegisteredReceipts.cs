using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotJoes.Infrastructure.ComplianceConsumer.Migrations
{
    /// <inheritdoc />
    public partial class AddComplianceVendorRegisteredReceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "compliance_vendor_registered_receipts",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    event_version = table.Column<int>(type: "integer", nullable: false),
                    received_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    serialized_event_sha256 = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compliance_vendor_registered_receipts", x => x.event_id);
                    table.CheckConstraint("ck_compliance_receipts_event_type", "\"event_type\" = 'VendorRegistered'");
                    table.CheckConstraint("ck_compliance_receipts_event_version", "\"event_version\" = 1");
                    table.CheckConstraint("ck_compliance_receipts_sha256", "octet_length(\"serialized_event_sha256\") = 32");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compliance_vendor_registered_receipts");
        }
    }
}
