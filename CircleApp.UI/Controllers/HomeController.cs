namespace CircleApp.UI.Controllers;

using System.Security.Claims;
using Base;
using Domain.Entities;
using Domain.ViewModels.Home;
using Infrastructure.Persistence.Constants;
using Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Services.Interfaces;


[Authorize]
public class HomeController : BaseController {

    private readonly AppDbContext _appDbContext;

    private readonly ILogger<HomeController> _logger;

    private readonly IPostService _postService;


    private readonly INotificationService _notificationService;

    public int LoggedInUserId { get; set; }

    public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext, IPostService postService, INotificationService notificationService)
    {
        _postService = postService;
        _logger = logger;
        _appDbContext = appDbContext;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index()
    {
        LoggedInUserId = GetUserId();
        List<Post>? posts = await _postService.GetAllPosts(LoggedInUserId)!;

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
        LoggedInUserId = GetUserId();
        await _postService.CreatePost(post, LoggedInUserId);


        return RedirectToAction("Index");
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> LikePost(LikePostVm likePost)
    {
        LoggedInUserId = GetUserId();
        var response = await _postService.LikePost(LoggedInUserId, likePost.PostId);
        var post = await _postService.GetPostByIdAsync(likePost.PostId);

        if (response.Success && response.SendNotification){
            await _notificationService.AddNewNotification(post.UserId, NotificationTypes.Like, GetUserFullName());
        }


        return PartialView("Post/_Like", post);
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
        var post = await _postService.GetPostByIdAsync(commentPostVm.PostId);
        var comments = post.Comments.ToList();
        await _notificationService.AddNewNotification(post.UserId, NotificationTypes.Comment, GetUserFullName());

        return PartialView("Post/_CommentsContent", comments);
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
        var response = await _postService.BookmarkPost(LoggedInUserId, bookmarkPostVm.PostId);
        var post = await _postService.GetPostByIdAsync(bookmarkPostVm.PostId);

        if (response){
            await _notificationService.AddNewNotification(post.UserId, NotificationTypes.Bookmark, GetUserFullName());
        }

        return PartialView("Post/_BookmarkIcon", post);
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
