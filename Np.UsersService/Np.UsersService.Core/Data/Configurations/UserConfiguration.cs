using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Np.UsersService.Core.Models.Users;

namespace Np.UsersService.Core.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.HasAlternateKey(u => u.IdentityId);

        builder.HasIndex(u=> u.Username).IsUnique();
        builder.HasIndex(u=> u.Email).IsUnique();

        builder.Property(u=> u.Username).IsRequired();
        builder.Property(u=> u.Email).IsRequired();
        builder.Property(u=> u.Id);
        builder.Property(u=> u.IdentityId);
    }
}
