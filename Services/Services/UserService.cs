namespace Services.Services;

using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Interfaces;
using Microsoft.EntityFrameworkCore;


public class UserService : IUserService {

    private readonly AppDbContext _context;

    private readonly IPostService _postService;

    public UserService(AppDbContext context, IPostService postService)
    {
        _context = context;
        _postService = postService;
    }

    public async Task<User?> GetUserDataAsync(int loggedInUserId)
    {
        var loggedInUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == loggedInUserId);

        return loggedInUser;
    }

    public async Task UpdateUserProfilePicture(int loggedInUserId, string profilePictureUrl)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == loggedInUserId);

        if (user != null){
            user.ProfilePictureUrl = profilePictureUrl;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Post>> GetUserPostsByIdAsync(int userId)
    {
        var allPosts = await _context.Posts
            .Where(n => (n.UserId == userId && (!n.IsPrivate || n.UserId == userId) && n.Reports.Count < 5 && !n.IsDeleted))
            .Include(n => n.User)
            .Include(n => n.Likes)
            .Include(n => n.Bookmarks)
            // .Include(n => n.Comments).ThenInclude(n => n.User)
            .Include(n => n.Reports)
            .OrderByDescending(n => n.DateCreated)
            .ToListAsync();

        if (allPosts == null){
            return new List<Post>();
        }

        return allPosts;
    }

}
