using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HotJoes.Infrastructure.CommunityConsumer.Migrations;

[DbContext(typeof(CommunityReceiptDbContext))]
[Migration("20260923190000_InitialCommunityReceiptSchema")]
public sealed class InitialCommunityReceiptSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "community");
        migrationBuilder.CreateTable(
            name: "community_participation_receipts", schema: "community",
            columns: table => new
            {
                event_id = table.Column<Guid>(type: "uuid", nullable: false),
                event_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                event_version = table.Column<int>(type: "integer", nullable: false),
                community_participation_id = table.Column<Guid>(type: "uuid", nullable: false),
                vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                received_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                serialized_event_sha256 = table.Column<byte[]>(type: "bytea", nullable: false)
            },
            constraints: table => table.PrimaryKey("pk_community_participation_receipts", x => x.event_id));
        migrationBuilder.CreateIndex(name: "ix_community_receipts_participation_id", schema: "community", table: "community_participation_receipts", column: "community_participation_id");
        migrationBuilder.CreateIndex(name: "ix_community_receipts_vendor_id", schema: "community", table: "community_participation_receipts", column: "vendor_id");
    }
    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(name: "community_participation_receipts", schema: "community");
}
