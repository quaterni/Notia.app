
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notia.Desctop.Services.Sessions.Abstractions;
using System;

namespace Notia.Desctop.Data.Configurations;

internal class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(s=> s.SessionId);

        builder.HasIndex(s=> s.LastAccess);

        builder.Property(s => s.AccessToken)
               .IsRequired();
        builder.Property(s => s.RefreshToken)
               .IsRequired();

        builder
            .Property(s => s.LastAccess)
            .HasConversion(src => src.UtcDateTime, dst => DateTime.SpecifyKind(dst, DateTimeKind.Utc))
            .IsRequired();
    }
}
