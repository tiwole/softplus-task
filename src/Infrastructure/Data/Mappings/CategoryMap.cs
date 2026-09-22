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

        builder.HasIndex(category => category.Name).IsUnique();
        
        builder.Property(category => category.Name)
            .HasMaxLength(128)
            .IsRequired();
    }
}
