namespace Services.Services;

using Domain.DTO;
using Domain.Entities;
using Domain.ViewModels.Home;
using Infrastructure.Persistence.Constants;
using Infrastructure.Persistence.DbContexts;
using Infrastructure.Persistence.Helpers;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


public class PostService : IPostService {

    private readonly AppDbContext _context;

    private readonly INotificationService _notificationService;

    public PostService(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<List<Post>>? GetAllPosts(int loggedUserId)
    {
        List<Post> allPosts = await _context.Posts
            .Where(n => (!n.IsPrivate || n.UserId == loggedUserId) && n.Reports.Count < 5 && !n.IsDeleted)
            .Include(n => n.User)
            .Include(n => n.Likes)
            .Include(n => n.Bookmarks)
            .Include(n => n.Comments).ThenInclude(n => n.User)
            .Include(n => n.Reports)
            .OrderByDescending(n => n.DateCreated)
            .ToListAsync();

        return allPosts;
    }

    public async Task<List<Post>> GetReportedPosts()
    {
        var posts = await _context.Posts
            .Where(post => !post.IsDeleted && post.NrOfReports > 3)
            .ToListAsync();

        return posts;
    }


    public async Task<Post> GetPostByIdAsync(int postId)
    {
        var postDb = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Bookmarks)
            .Include(p => p.Comments).ThenInclude(n => n.User)
            .FirstOrDefaultAsync(p => p.PostId == postId);

        return postDb!;
    }

    public async Task CreatePost(PostVM post, int loggedUserId)
    {
        var newPost = new Post
        {
            Content = post.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = "",
            UserId = loggedUserId,
            NrOfReports = 0
        };

        if (post.Image != null && post.Image.Length > 0){
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

        await _context.Posts.AddAsync(newPost);
        await _context.SaveChangesAsync();

        // find the hashtags of the post content 
        List<string> hashtags = HashtagHelper.ExtractHashtags(post.Content);


        if (hashtags != new List<string>())
            foreach (var hashtag in hashtags){
                var hashtagDb = await _context.Hashtags.FirstOrDefaultAsync(n => n.Name == hashtag);

                if (hashtagDb != null){
                    hashtagDb.Posts.Add(newPost);
                    hashtagDb.Count += 1;
                    hashtagDb.DateUpdated = DateTime.UtcNow;
                    _context.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }
                else{
                    var newHashtag = new Hashtag
                    {
                        Name = hashtag,
                        DateUpdated = DateTime.UtcNow,
                        DateCreated = DateTime.UtcNow,
                        Count = 1
                    };

                    newHashtag.Posts.Add(newPost);
                    await _context.Hashtags.AddAsync(newHashtag);
                    await _context.SaveChangesAsync();
                }
            }
    }

    public async Task<NotificationResponse> LikePost(int loggedInUserId, int postId)
    {
        const int maxRetries = 5;
        int retryCount = 0;

        var notificationResponse = new NotificationResponse()
        {
            Success = false,
            SendNotification = false
        };

        while (retryCount < maxRetries){
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try{
                var liked = await _context.Likes
                    .FirstOrDefaultAsync(l => l.UserId == loggedInUserId && l.PostId == postId);

                if (liked != null){
                    _context.Likes.Remove(liked);
                    notificationResponse.Success = true;
                }
                else{
                    _context.Likes.Add(new Like { UserId = loggedInUserId, PostId = postId });
                    notificationResponse.Success = true;
                    notificationResponse.SendNotification = true;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return notificationResponse;
            }
            catch (DbUpdateConcurrencyException){
                await transaction.RollbackAsync();
                retryCount++;

                if (retryCount >= maxRetries)
                    throw;

                // Reset DbContext state
                foreach (var entry in _context.ChangeTracker.Entries())
                    entry.State = EntityState.Detached;
            }
        }

        return notificationResponse;
    }

    public async Task AddComment(Comment comment)
    {
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteComment(int loggedUserId, int commentId)
    {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);

        if (comment != null){
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> BookmarkPost(int loggedInUserId, int postId)
    {
        var bookmark = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == loggedInUserId && b.PostId == postId);

        if (bookmark != null){
            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();

            return false;
        }
        else{
            var newBookmark = new Bookmark
            {
                PostId = postId,
                UserId = loggedInUserId,
                DateCreated = DateTime.UtcNow
            };

            _context.Bookmarks.Add(newBookmark);
            await _context.SaveChangesAsync();

            return true;
        }

        return false;
    }

    public async Task TogglePostVisibility(int loggedUserId, int postId)
    {
        var post = await _context.Posts
            .FirstOrDefaultAsync(p => p.UserId == loggedUserId && p.PostId == postId);

        if (post != null){
            post.IsPrivate = !post.IsPrivate;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ReportPost(int loggedInUserId, int postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(p => p.UserId == loggedInUserId && p.PostId == postId);

        if (post == null){
            return;
        }


        var reported = await _context.Reports
            .FirstOrDefaultAsync(r => r.UserId == loggedInUserId && r.PostId == postId);


        if (reported == null){
            var report = new Report
            {
                PostId = postId,
                UserId = loggedInUserId,
                DateCreated = DateTime.UtcNow
            };

            post.NrOfReports += 1;
            _context.Posts.Update(post);
            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeletePost(int loggedInUesrId, int postId)
    {
        var post = _context.Posts.FirstOrDefault(p => p.PostId == postId && loggedInUesrId == p.UserId);

        if (post != null){
            post.IsDeleted = true;
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            // fix the hashtag implication 
            List<string> hashtagsOfPost = HashtagHelper.ExtractHashtags(post.Content);

            if (hashtagsOfPost != new List<string>())
                foreach (var hash in hashtagsOfPost){
                    var hashtagFromDb = await _context.Hashtags.FirstOrDefaultAsync(h => h.Name == hash);

                    if (hashtagFromDb != null){
                        hashtagFromDb.Count -= 1;
                        hashtagFromDb.Posts.Remove(post);
                        _context.Hashtags.Update(hashtagFromDb);
                        await _context.SaveChangesAsync();
                    }
                }
        }
    }

    public async Task DismissPostReports(int postId)
    {
        var post = await GetPostByIdAsync(postId);
        post.NrOfReports = 0;
        var reports = await _context.Reports.Where(report => report.PostId == postId).ToListAsync();
        _context.Reports.RemoveRange(reports);
        _context.Posts.Update(post);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveReportedPost(int postId)
    {
        var post = await _context.Posts.FirstOrDefaultAsync(r => r.PostId == postId);

        if (post != null){
            post.IsDeleted = true;
            _context.Posts.Update(post);
            await _notificationService.AddNewNotification(post.UserId, NotificationTypes.Post, post.User.FullName);
            await _context.SaveChangesAsync();
        }
    }

}
