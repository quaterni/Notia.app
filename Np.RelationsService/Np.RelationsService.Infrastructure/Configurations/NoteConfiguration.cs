using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Np.RelationsService.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Np.RelationsService.Infrastructure.Configurations
{
    internal class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("notes");

            builder.HasKey(x => x.Id);

            builder
                .Property<uint>("Version")
                .IsRowVersion();
        }
    }
}
