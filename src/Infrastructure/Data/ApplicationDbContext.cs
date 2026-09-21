using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

    public DbSet<TodoTask> Tasks => Set<TodoTask>();

    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasIndex(user => user.Email).IsUnique();
            entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(category => category.Name).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<TodoTask>(entity =>
        {
            entity.Property(task => task.Title).HasMaxLength(256).IsRequired();
            entity.Property(task => task.Description).HasMaxLength(2000);

            entity.HasOne(task => task.User)
                .WithMany(user => user.Tasks)
                .HasForeignKey(task => task.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(task => task.Category)
                .WithMany(category => category.Tasks)
                .HasForeignKey(task => task.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
