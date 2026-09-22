using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings;

public class ApplicationUserMap : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUser");
        builder.HasKey(user => user.Id);
        
        builder.HasIndex(user => user.Email).IsUnique();
        
        builder.Property(user => user.Email)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(user => user.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();
    }
}