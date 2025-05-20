namespace CircleApp.UI.Controllers;

using System.Security.Claims;
using Base;
using Domain.Entities;
using Domain.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;


[Authorize]
public class FavoritesController : BaseController {

    private readonly IFavoritesService _favoritesService;

    private readonly IPostService _postService;

    public int LoggedInUserId { get; set; }


    public FavoritesController(IFavoritesService favoritesService, IPostService postService)
    {
        _favoritesService = favoritesService;
        _postService = postService;
    }


    public async Task<IActionResult> Index()
    {
        LoggedInUserId = GetUserId();
        List<Post> bookmarkedPosts = await _favoritesService.GetFavoritePostsAsync(LoggedInUserId);

        return View(bookmarkedPosts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStatus(PostVM post)
    {
        LoggedInUserId = GetUserId();
        await _postService.CreatePost(post, LoggedInUserId);


        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> LikePost(LikePostVm likePost)
    {
        LoggedInUserId = GetUserId();
        await _postService.LikePost(LoggedInUserId, likePost.PostId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(CommentPostVm commentPostVm)
    {
        LoggedInUserId = GetUserId();

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
        LoggedInUserId = GetUserId();
        await _postService.DeleteComment(LoggedInUserId, deleteCommentVm.CommentId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> BookmarkPost(BookmarkPostVm bookmarkPostVm)
    {
        LoggedInUserId = GetUserId();
        await _postService.BookmarkPost(LoggedInUserId, bookmarkPostVm.PostId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> PostVisibility(PostVisibilityVm postVisibilityVm)
    {
        LoggedInUserId = GetUserId();
        await _postService.TogglePostVisibility(LoggedInUserId, postVisibilityVm.PostId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ReportPost(ReportPostVm reportPostVm)
    {
        LoggedInUserId = GetUserId();
        await _postService.ReportPost(LoggedInUserId, reportPostVm.PostId);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeletePost(DeletePostVm deletePostVm)
    {
        LoggedInUserId = GetUserId();
        await _postService.DeletePost(LoggedInUserId, deletePostVm.PostId);

        return RedirectToAction("Index");
    }

}
