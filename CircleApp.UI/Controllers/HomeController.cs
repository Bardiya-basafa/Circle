namespace CircleApp.UI.Controllers;

using Domain.Entities;
using Domain.ViewModels.Home;
using Infrastructure.Persistence.DbContexts;
using Infrastructure.Persistence.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class HomeController : Controller {

    public int loggedInUesr = 1;


    private readonly AppDbContext _appDbContext;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbContext = appDbContext;
    }

    public async Task<IActionResult> Index()
    {
        IndexPageObject indexPageObject = new IndexPageObject();

        List<Post> allPosts = await _appDbContext.Posts
            .Where(n => (!n.IsPrivate || n.UserId == loggedInUesr) && n.Reports.Count < 5 && !n.IsDeleted)
            .Include(n => n.User)
            .Include(n => n.Likes)
            .Include(n => n.Bookmarks)
            .Include(n => n.Comments).ThenInclude(n => n.User)
            .Include(n => n.Reports)
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

        // find the hashtags of the post content 
        var hashtags = HashtagHelper.ExtractHashtags(post.Content);


        if (hashtags != new List<string>()){
            foreach (var hashtag in hashtags){
                var hashtagDb = await _appDbContext.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);

                if (hashtagDb != null){
                    hashtagDb.Posts.Add(newPost);
                    hashtagDb.Count += 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;
                    _appDbContext.Update(hashtagDb);
                    await _appDbContext.SaveChangesAsync();
                }
                else{
                    var newHashtag = new Hashtag
                    {
                        Name = hashtag,
                        DateUpdated = DateTime.UtcNow,
                        DateCreated = DateTime.UtcNow,
                        Count = 1,
                    };

                    newHashtag.Posts.Add(newPost);
                    await _appDbContext.Hashtags.AddAsync(newHashtag);
                    await _appDbContext.SaveChangesAsync();
                }
            }
        }


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

    [HttpPost]
    public async Task<IActionResult> DeletePost(DeletePostVm deletePostVm)
    {
        var post = _appDbContext.Posts.FirstOrDefault(p => p.PostId == deletePostVm.PostId && loggedInUesr == p.UserId);

        if (post != null){
            post.IsDeleted = true;
            _appDbContext.Posts.Update(post);
            await _appDbContext.SaveChangesAsync();

            // fix the hashtag implication 
            var hashtagsOfPost = HashtagHelper.ExtractHashtags(post.Content);

            if (hashtagsOfPost != new List<string>()){
                foreach (var hash in hashtagsOfPost){
                    var hashtagFromDb = await _appDbContext.Hashtags.FirstOrDefaultAsync(h => h.Name == hash);

                    if (hashtagFromDb != null){
                        hashtagFromDb.Count -= 1;
                        hashtagFromDb.Posts.Remove(post);
                        _appDbContext.Hashtags.Update(hashtagFromDb);
                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
        }


        return RedirectToAction("Index");
    }

}
