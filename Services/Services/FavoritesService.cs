namespace Services.Services;

using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Interfaces;
using Microsoft.EntityFrameworkCore;


public class FavoritesService : IFavoritesService {

    private readonly AppDbContext _context;

    public FavoritesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Post>> GetFavoritePostsAsync(int loggedInUserId)
    {
        List<Post> favoritePosts = new List<Post>();

        favoritePosts = await _context.Posts
            .Where(p => p.Bookmarks.Where(b => b.UserId == loggedInUserId).Any() && !p.IsDeleted)
            .Include(n => n.User)
            .Include(n => n.Likes)
            .Include(n => n.Bookmarks)
            .Include(n => n.Comments).ThenInclude(n => n.User)
            .Include(n => n.Reports)
            .OrderByDescending(n => n.DateCreated)
            .ToListAsync();

        return favoritePosts;
    }

}
