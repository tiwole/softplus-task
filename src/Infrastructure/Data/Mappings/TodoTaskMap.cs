using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Mappings;

public class TodoTaskMap : IEntityTypeConfiguration<TodoTask>
{
    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        builder.ToTable("Task");

        builder.HasKey(task => task.Id);
        
        builder.HasIndex(task => task.UserId);

        builder.HasOne<ApplicationUser>()
            .WithMany(user => user.Tasks)
            .HasForeignKey(task => task.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(task => task.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.Property(task => task.Title).HasMaxLength(256).IsRequired();
        builder.Property(task => task.IsCompleted).HasDefaultValue(false);
        builder.Property(task => task.CreatedAtUtc).HasDefaultValueSql("now()");
    }
}
