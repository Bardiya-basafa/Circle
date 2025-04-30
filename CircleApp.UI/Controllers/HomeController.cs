namespace CircleApp.UI.Controllers;

using Domain.Entities;
using Domain.ViewModels.Home;
using Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class HomeController : Controller {

    private readonly AppDbContext _appDbContext;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbContext = appDbContext;
    }

    public async Task<IActionResult> Index()
    {
        int loggedInUesr = 1;

        List<Post> allPosts = await _appDbContext.Posts
            .Where(n => !n.IsPrivate || n.UserId == loggedInUesr)
            .Include(n => n.User)
            .Include(n => n.Likes)
            .Include(n => n.Bookmarks)
            .Include(n => n.Comments).ThenInclude(n => n.User)
            .OrderByDescending(n => n.DateCreated)
            .ToListAsync();

        return View(allPosts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStatus(PostVM post)
    {
        var newPost = new Post
        {
            Content = post.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = "",
            UserId = 1,
            NrOfReports = 0
        };

        if (post.Image.Length > 0){
            var rootFoder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (post.Image.ContentType.Contains("image")){
                var rootImageFolderPath = Path.Combine(rootFoder, "postimages");
                Directory.CreateDirectory(rootImageFolderPath);
                var fileName = Guid.NewGuid() + Path.GetExtension(post.Image.FileName);
                var filePath = Path.Combine(rootImageFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create)){
                    await post.Image.CopyToAsync(stream);
                }

                newPost.ImageUrl = "/postimages/" + fileName;
            }
        }

        await _appDbContext.Posts.AddAsync(newPost);
        await _appDbContext.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> LikePost(LikePostVm likePost)
    {
        var loggedInUserId = 1;

        var liked = await _appDbContext.Likes
            .FirstOrDefaultAsync(l => l.UserId == loggedInUserId && l.PostId == likePost.PostId);

        if (liked != null){
            _appDbContext.Likes.Remove(liked);
            await _appDbContext.SaveChangesAsync();
        }
        else{
            var like = new Like
            {
                PostId = likePost.PostId,
                UserId = loggedInUserId
            };

            _appDbContext.Likes.Add(like);
            await _appDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(CommentPostVm commentPostVm)
    {
        var loggedInUserId = 1;

        var comment = new Comment()
        {
            PostId = commentPostVm.PostId,
            UserId = loggedInUserId,
            Content = commentPostVm.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
        };

        _appDbContext.Comments.Add(comment);
        await _appDbContext.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteComment(DeleteCommentVm deleteCommentVm)
    {
        int loggedInUserId = 1;
        var comment = _appDbContext.Comments.FirstOrDefault(c => c.Id == deleteCommentVm.CommentId);

        if (comment != null){
            _appDbContext.Comments.Remove(comment);
            await _appDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> BookmarkPost(BookmarkPostVm bookmarkPostVm)
    {
        int loggedInUserId = 1;

        var bookmark = await _appDbContext.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == loggedInUserId && b.PostId == bookmarkPostVm.PostId);

        if (bookmark != null){
            _appDbContext.Bookmarks.Remove(bookmark);
            await _appDbContext.SaveChangesAsync();
        }
        else{
            Bookmark newBookmark = new Bookmark()
            {
                PostId = bookmarkPostVm.PostId,
                UserId = loggedInUserId,
                DateCreated = DateTime.UtcNow,
            };

            _appDbContext.Bookmarks.Add(newBookmark);
            await _appDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> PostVisibility(PostVisibilityVm postVisibilityVm)
    {
        int loggedInUserId = 1;

        var post = await _appDbContext.Posts
            .FirstOrDefaultAsync(p => p.UserId == loggedInUserId && p.PostId == postVisibilityVm.PostId);

        if (post != null){
            post.IsPrivate = !post.IsPrivate;
            _appDbContext.Posts.Update(post);
            await _appDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ReportPost(ReportPostVm reportPostVm)
    {
        int loggedInUserId = 2;
        var post = await _appDbContext.Posts.FirstOrDefaultAsync(p => p.UserId == loggedInUserId && p.PostId == reportPostVm.PostId);

        if (post != null){
            return RedirectToAction("Index");
        }

        var reported = await _appDbContext.Reports
            .FirstOrDefaultAsync(r => r.UserId == loggedInUserId && r.PostId == reportPostVm.PostId);


        if (reported == null){
            Report report = new Report()
            {
                PostId = reportPostVm.PostId,
                UserId = loggedInUserId,
                DateCreated = DateTime.UtcNow,
            };

            await _appDbContext.Reports.AddAsync(report);
            await _appDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index");

        ;
    }

}
