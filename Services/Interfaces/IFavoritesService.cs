namespace Services.Interfaces;

using Domain.Entities;


public interface IFavoritesService {

    public Task<List<Post>> GetFavoritePostsAsync(int loggedInUserId);

}
