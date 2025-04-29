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
        List<Post> allPosts = await _appDbContext.Posts
            .Include(n => n.User)
            .Include(n => n.Likes)
            .Include(n => n.Comments).ThenInclude(n => n.User)
            .Include(n => n.Bookmarks).ThenInclude(n => n.User)
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

        var liked = _appDbContext.Likes
            .FirstOrDefault(l => l.UserId == loggedInUserId && l.PostId == likePost.PostId);

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

}
