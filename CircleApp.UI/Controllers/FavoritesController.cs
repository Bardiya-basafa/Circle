using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.Controllers;

using Domain.Entities;
using Domain.ViewModels.Home;
using Services.Interfaces;
using Services.Services;


public class FavoritesController : Controller {

    public int LoggedInUserId { get; set; } = 1;

    private readonly IFavoritesService _favoritesService;

    private readonly IPostService _postService;

    public FavoritesController(IFavoritesService favoritesService, IPostService postService)
    {
        _favoritesService = favoritesService;
        _postService = postService;
    }

    public async Task<IActionResult> Index()
    {
        List<Post> bookmarkedPosts = await _favoritesService.GetFavoritePostsAsync(LoggedInUserId);

        return View(bookmarkedPosts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStatus(PostVM post)
    {
        await _postService.CreatePost(post);


        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> LikePost(LikePostVm likePost)
    {
        await _postService.LikePost(LoggedInUserId, likePost.PostId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(CommentPostVm commentPostVm)
    {
        var comment = new Comment
        {
            PostId = commentPostVm.PostId,
            UserId = LoggedInUserId,
            Content = commentPostVm.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow
        };

        await _postService.AddComment(comment);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(DeleteCommentVm deleteCommentVm)
    {
        await _postService.DeleteComment(LoggedInUserId, deleteCommentVm.CommentId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> BookmarkPost(BookmarkPostVm bookmarkPostVm)
    {
        await _postService.BookmarkPost(LoggedInUserId, bookmarkPostVm.PostId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> PostVisibility(PostVisibilityVm postVisibilityVm)
    {
        await _postService.TogglePostVisibility(LoggedInUserId, postVisibilityVm.PostId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ReportPost(ReportPostVm reportPostVm)
    {
        await _postService.ReportPost(LoggedInUserId, reportPostVm.PostId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeletePost(DeletePostVm deletePostVm)
    {
        await _postService.DeletePost(LoggedInUserId, deletePostVm.PostId);

        return RedirectToAction("Index");
    }

}
