using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Np.RelationsService.Domain.Relations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Infrastructure.Configurations
{
    internal class RelationConfiguration : IEntityTypeConfiguration<Relation>
    {
        public void Configure(EntityTypeBuilder<Relation> builder)
        {
            builder.ToTable("relations");

            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.Incoming)
                .WithMany()
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Outgoing)
                .WithMany()
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property<uint>("Version")
                .IsRowVersion();
        }
    }
}
