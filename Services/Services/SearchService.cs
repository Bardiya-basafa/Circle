namespace Services.Services;

using Domain.DTO;
using Infrastructure.Persistence.DbContexts;
using Interfaces;
using Microsoft.EntityFrameworkCore;


public class SearchService : ISearchService {

    private readonly AppDbContext _context;

    public SearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SearchResponse> Search(string searchString, int userId)
    {
        var posts = await _context.Posts.Where(post => post.Content.Contains(searchString)).ToListAsync();
        var users = await _context.Users.Where(user => user.FullName.Contains(searchString)).ToListAsync();

        var userFriends = await _context.FriendShips.Where(friendship => (friendship.SenderId == userId && friendship.Receiver.FullName.Contains(searchString)) || (friendship.ReceiverId == userId && friendship.Sender.FullName.Contains(searchString)))
            .Select(friendship =>
                friendship.SenderId == userId
                    ? friendship.Receiver
                    : friendship.Sender)
            .ToListAsync();

        var result = new SearchResponse()
        {
            Posts = posts,
            Users = users,
            UserFriends = userFriends
        };

        return result;
    }

}
