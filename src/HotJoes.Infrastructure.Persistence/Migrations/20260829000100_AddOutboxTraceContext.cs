using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotJoes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxTraceContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "trace_parent",
                table: "vendor_registration_outbox",
                type: "character varying(55)",
                maxLength: 55,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trace_state",
                table: "vendor_registration_outbox",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "trace_parent",
                table: "vendor_registration_outbox");

            migrationBuilder.DropColumn(
                name: "trace_state",
                table: "vendor_registration_outbox");
        }
    }
}
