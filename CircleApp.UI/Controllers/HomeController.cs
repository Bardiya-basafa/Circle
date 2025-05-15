namespace CircleApp.UI.Controllers;

using Domain.Entities;
using Domain.ViewModels.Home;
using Infrastructure.Persistence.DbContexts;
using Infrastructure.Persistence.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;


public class HomeController : Controller {

    private readonly AppDbContext _appDbContext;

    private readonly IPostService _postService;

    private readonly ILogger<HomeController> _logger;

    public int LoggedInUserId = 1;

    public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext, IPostService postService)
    {
        _postService = postService;
        _logger = logger;
        _appDbContext = appDbContext;
    }

    public async Task<IActionResult> Index()
    {
        var posts = await _postService.GetAllPosts(LoggedInUserId)!;

        return View(posts);
    }

    public async Task<IActionResult> Details(int postId)
    {
        var post = await _postService.GetPostByIdAsync(postId);

        return View("Details", post);
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
