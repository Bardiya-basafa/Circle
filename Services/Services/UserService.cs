namespace Services.Services;

using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Interfaces;
using Microsoft.EntityFrameworkCore;


public class UserService : IUserService {

    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
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

}
