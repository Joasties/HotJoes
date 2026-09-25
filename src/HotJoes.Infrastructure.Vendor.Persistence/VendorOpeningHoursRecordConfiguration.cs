using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotJoes.Infrastructure.Vendor.Persistence;

internal sealed class VendorOpeningHoursRecordConfiguration
    : IEntityTypeConfiguration<VendorOpeningHoursRecord>
{
    public void Configure(EntityTypeBuilder<VendorOpeningHoursRecord> builder)
    {
        builder.ToTable("vendor_opening_hours", table =>
        {
            table.HasCheckConstraint(
                "ck_vendor_opening_hours_day",
                "\"day\" IN ('monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday')");
            table.HasCheckConstraint(
                "ck_vendor_opening_hours_state",
                "(\"is_closed\" AND NOT \"is_open_all_day\" AND \"start_time\" IS NULL AND \"end_time\" IS NULL) OR " +
                "(NOT \"is_closed\" AND \"is_open_all_day\" AND \"start_time\" IS NULL AND \"end_time\" IS NULL) OR " +
                "(NOT \"is_closed\" AND NOT \"is_open_all_day\" AND \"start_time\" IS NOT NULL AND \"end_time\" IS NOT NULL AND \"start_time\" <> \"end_time\")");
        });

        builder.HasKey(record => new { record.VendorId, record.Day });
        builder.Property(record => record.VendorId).HasColumnName("vendor_id");
        builder.Property(record => record.Day)
            .HasColumnName("day").HasMaxLength(9).IsRequired();
        builder.Property(record => record.IsClosed)
            .HasColumnName("is_closed").IsRequired();
        builder.Property(record => record.IsOpenAllDay)
            .HasColumnName("is_open_all_day").IsRequired();
        builder.Property(record => record.StartTime)
            .HasColumnName("start_time").HasColumnType("time without time zone");
        builder.Property(record => record.EndTime)
            .HasColumnName("end_time").HasColumnType("time without time zone");

        builder.HasOne(record => record.Vendor)
            .WithMany(vendor => vendor.WeeklyOpeningHours)
            .HasForeignKey(record => record.VendorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
