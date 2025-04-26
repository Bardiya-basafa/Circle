namespace Infrastructure.Persistence.DbContexts;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;


public class AppDbContext : DbContext {

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }

    public DbSet<User> Users { get; set; }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from assembly
        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);
    }

}
