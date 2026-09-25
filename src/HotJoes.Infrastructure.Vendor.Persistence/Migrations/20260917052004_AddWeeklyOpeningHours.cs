using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotJoes.Infrastructure.Vendor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyOpeningHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vendor_opening_hours",
                columns: table => new
                {
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    is_closed = table.Column<bool>(type: "boolean", nullable: false),
                    is_open_all_day = table.Column<bool>(type: "boolean", nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    end_time = table.Column<TimeOnly>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vendor_opening_hours", x => new { x.vendor_id, x.day });
                    table.CheckConstraint("ck_vendor_opening_hours_day", "\"day\" IN ('monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday')");
                    table.CheckConstraint("ck_vendor_opening_hours_state", "(\"is_closed\" AND NOT \"is_open_all_day\" AND \"start_time\" IS NULL AND \"end_time\" IS NULL) OR (NOT \"is_closed\" AND \"is_open_all_day\" AND \"start_time\" IS NULL AND \"end_time\" IS NULL) OR (NOT \"is_closed\" AND NOT \"is_open_all_day\" AND \"start_time\" IS NOT NULL AND \"end_time\" IS NOT NULL AND \"start_time\" <> \"end_time\")");
                    table.ForeignKey(
                        name: "FK_vendor_opening_hours_vendor_registrations_vendor_id",
                        column: x => x.vendor_id,
                        principalTable: "vendor_registrations",
                        principalColumn: "vendor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                """
                INSERT INTO vendor_opening_hours
                    (vendor_id, day, is_closed, is_open_all_day, start_time, end_time)
                SELECT registrations.vendor_id, days.day, FALSE, FALSE,
                       registrations.opening_hours_start,
                       registrations.opening_hours_end
                FROM vendor_registrations AS registrations
                CROSS JOIN (
                    VALUES ('monday'), ('tuesday'), ('wednesday'),
                           ('thursday'), ('friday'), ('saturday'), ('sunday')
                ) AS days(day);
                """);

            migrationBuilder.DropColumn(
                name: "opening_hours_end",
                table: "vendor_registrations");

            migrationBuilder.DropColumn(
                name: "opening_hours_start",
                table: "vendor_registrations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "opening_hours_end",
                table: "vendor_registrations",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.Sql(
                """
                UPDATE vendor_registrations AS registrations
                SET opening_hours_start = hours.start_time,
                    opening_hours_end = hours.end_time
                FROM vendor_opening_hours AS hours
                WHERE hours.vendor_id = registrations.vendor_id
                  AND hours.day = 'monday'
                  AND NOT hours.is_closed
                  AND NOT hours.is_open_all_day;
                """);

            migrationBuilder.DropTable(
                name: "vendor_opening_hours");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "opening_hours_start",
                table: "vendor_registrations",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
