using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.RootEntries;

namespace Np.RelationsService.Infrastructure.Configurations
{
    internal class RootEntryConfiguration : IEntityTypeConfiguration<RootEntry>
    {
        public void Configure(EntityTypeBuilder<RootEntry> builder)
        {
            builder.ToTable("root_entries");

            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.Note)
                .WithOne()
                .HasPrincipalKey<Note>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);


            builder
                .Property<uint>("Version")
                .IsRowVersion();
        }
    }
}
