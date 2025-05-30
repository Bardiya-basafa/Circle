namespace Services.Interfaces;

using Domain.DTO;
using Domain.Entities;
using Domain.ViewModels.Home;


public interface IPostService {

    Task<List<Post>>? GetAllPosts(int loggedUserId);

    Task<List<Post>> GetReportedPosts();

    Task<Post> GetPostByIdAsync(int postId);

    Task CreatePost(PostVM post, int loggedUserId);

    Task<NotificationResponse> LikePost(int loggedUserId, int postId);

    Task AddComment(Comment comment);

    Task DeleteComment(int loggedUserId, int commentId);

    Task<bool> BookmarkPost(int loggedUserId, int postId);

    Task TogglePostVisibility(int loggedUserId, int postId);

    Task ReportPost(int loggedUserId, int postId);

    Task DeletePost(int loggedUserId, int postId);

    Task DismissPostReports(int postId);

    Task RemoveReportedPost(int postId);

}
