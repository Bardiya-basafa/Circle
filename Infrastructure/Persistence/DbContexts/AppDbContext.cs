namespace Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Post> Posts { get; set; }
    
}
