namespace Infrastructure.Persistence.DbContexts;

using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int> {

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Like> Likes { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Bookmark> Bookmarks { get; set; }

    public DbSet<Report> Reports { get; set; }

    public DbSet<Story> Stories { get; set; }

    public DbSet<Hashtag> Hashtags { get; set; }


    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from assembly

        // configuration for the users table 
        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Stories)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);


        // configuration for the likes table 
        modelBuilder.Entity<Like>()
            .HasKey(l => new { l.PostId, l.UserId });

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Post)
            .WithMany(p => p.Likes)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.User)
            .WithMany(u => u.Likes)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasOne(l => l.Story)
            .WithMany(s => s.Likes)
            .HasForeignKey(l => l.StoryId)
            .OnDelete(DeleteBehavior.NoAction);

        // configuration for the comments table 
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        // configuration the relations for bookmarks table
        modelBuilder.Entity<Bookmark>()
            .HasKey(b => new { b.PostId, b.UserId });

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.Post)
            .WithMany(p => p.Bookmarks)
            .HasForeignKey(b => b.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Bookmark>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bookmarks)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // relations for reports
        modelBuilder.Entity<Report>()
            .HasKey(r => new { r.PostId, r.UserId });

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Post)
            .WithMany(p => p.Reports)
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // hashtag relations
        modelBuilder.Entity<Post>()
            .HasMany(p => p.Hashtags)
            .WithMany(h => h.Posts)
            .UsingEntity<Dictionary<string, object>>(
            "PostHashtags",
            configureRight: j => j.HasOne<Hashtag>().WithMany()
                .HasForeignKey("HashtagId")
                .OnDelete(DeleteBehavior.NoAction),// No action when Hashtag is deleted
            configureLeft: j => j.HasOne<Post>().WithMany()
                .HasForeignKey("PostId")
                .OnDelete(DeleteBehavior.NoAction),// No action when Post is deleted
            configureJoinEntityType: j => j.ToTable("PostHashtags")
            );

        base.OnModelCreating(modelBuilder);

        // customize  entity tables names 
        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

    }

}
