using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Np.UsersService.Core.Messaging.Outbox.Models;

namespace Np.UsersService.Core.Data.Configurations;

public class OutboxEntryConfiguration : IEntityTypeConfiguration<OutboxEntry>
{
    public void Configure(EntityTypeBuilder<OutboxEntry> builder)
    {
        builder.ToTable("outbox_entries");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x=> x.RefreshTime);

        builder.Property(x => x.Name);
        builder.Property(x => x.Data);

        builder.Property(x => x.RefreshTime)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? 
                    src : 
                    DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ?
                    dst : 
                    DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            );

        builder.Property(x => x.Created)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ?
                    src :
                    DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ?
                    dst :
                    DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            );
    }
}
