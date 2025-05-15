namespace Services.Interfaces;

using Domain.Entities;
using Domain.ViewModels.Home;


public interface IPostService {

    Task<List<Post>>? GetAllPosts(int loggedUserId);

    Task<Post> GetPostByIdAsync(int postId);

    Task CreatePost(PostVM post);

    Task LikePost(int loggedUserId, int postId);

    Task AddComment(Comment comment);

    Task DeleteComment(int loggedUserId, int commentId);

    Task BookmarkPost(int loggedUserId, int postId);

    Task TogglePostVisibility(int loggedUserId, int postId);

    Task ReportPost(int loggedUserId, int postId);

    Task DeletePost(int loggedUserId, int postId);

}
