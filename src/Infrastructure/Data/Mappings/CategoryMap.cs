using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings;

public class CategoryMap : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Category");

        builder.HasKey(category => category.Id);
        
        builder.HasOne(category => category.User)
            .WithMany(user => user.Categories)
            .HasForeignKey(category => category.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(category => new
        {
            category.UserId,
            category.Name
        }).IsUnique();
        
        builder.Property(category => category.Name)
            .HasMaxLength(128)
            .IsRequired();
    }
}
