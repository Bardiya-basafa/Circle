namespace Services.Interfaces;

using Domain.Entities;


public interface IUserService {

    public Task<User?> GetUserDataAsync(int loggedInUserId);

    public Task UpdateUserProfilePicture(int loggedInUserId, string profilePictureUrl);

    public Task<List<Post>> GetUserPostsByIdAsync(int userId);

}
