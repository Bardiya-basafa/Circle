namespace Services.Services;

using Domain.Entities;
using Domain.ViewModels.Home;
using Infrastructure.Persistence.DbContexts;
using Infrastructure.Persistence.Helpers;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


public class PostService : IPostService {

    private readonly AppDbContext _appDbContext;

    public PostService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<Post>>? GetAllPosts(int loggedUserId)
    {
        List<Post> allPosts = await _appDbContext.Posts
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

    public async Task<Post> GetPostByIdAsync(int postId)
    {
        var postDb = await _appDbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Bookmarks)
            .Include(p => p.Comments).ThenInclude(n => n.User)
            .FirstOrDefaultAsync(p => p.PostId == postId);

        return postDb!;
    }

    public async Task CreatePost(PostVM post)
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
        List<string> hashtags = HashtagHelper.ExtractHashtags(post.Content);


        if (hashtags != new List<string>())
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
                        Count = 1
                    };

                    newHashtag.Posts.Add(newPost);
                    await _appDbContext.Hashtags.AddAsync(newHashtag);
                    await _appDbContext.SaveChangesAsync();
                }
            }
    }

    public async Task LikePost(int loggedInUserId, int postId)
    {
        var liked = await _appDbContext.Likes
            .FirstOrDefaultAsync(l => l.UserId == loggedInUserId && l.PostId == postId);

        if (liked != null){
            _appDbContext.Likes.Remove(liked);
            await _appDbContext.SaveChangesAsync();
        }
        else{
            var like = new Like
            {
                PostId = postId,
                UserId = loggedInUserId
            };

            _appDbContext.Likes.Add(like);
            await _appDbContext.SaveChangesAsync();
        }
    }

    public async Task AddComment(Comment comment)
    {
        _appDbContext.Comments.Add(comment);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task DeleteComment(int loggedUserId, int commentId)
    {
        var comment = _appDbContext.Comments.FirstOrDefault(c => c.Id == commentId);

        if (comment != null){
            _appDbContext.Comments.Remove(comment);
            await _appDbContext.SaveChangesAsync();
        }
    }

    public async Task BookmarkPost(int loggedInUserId, int postId)
    {
        var bookmark = await _appDbContext.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == loggedInUserId && b.PostId == postId);

        if (bookmark != null){
            _appDbContext.Bookmarks.Remove(bookmark);
            await _appDbContext.SaveChangesAsync();
        }
        else{
            var newBookmark = new Bookmark
            {
                PostId = postId,
                UserId = loggedInUserId,
                DateCreated = DateTime.UtcNow
            };

            _appDbContext.Bookmarks.Add(newBookmark);
            await _appDbContext.SaveChangesAsync();
        }
    }

    public async Task TogglePostVisibility(int loggedUserId, int postId)
    {
        var post = await _appDbContext.Posts
            .FirstOrDefaultAsync(p => p.UserId == loggedUserId && p.PostId == postId);

        if (post != null){
            post.IsPrivate = !post.IsPrivate;
            _appDbContext.Posts.Update(post);
            await _appDbContext.SaveChangesAsync();
        }
    }

    public async Task ReportPost(int loggedInUserId, int postId)
    {
        var post = await _appDbContext.Posts.FirstOrDefaultAsync(p => p.UserId == loggedInUserId && p.PostId == postId);

        if (post != null){
            return;
        }


        var reported = await _appDbContext.Reports
            .FirstOrDefaultAsync(r => r.UserId == loggedInUserId && r.PostId == postId);


        if (reported == null){
            var report = new Report
            {
                PostId = postId,
                UserId = loggedInUserId,
                DateCreated = DateTime.UtcNow
            };

            await _appDbContext.Reports.AddAsync(report);
            await _appDbContext.SaveChangesAsync();
        }
    }

    public async Task DeletePost(int loggedInUesrId, int postId)
    {
        var post = _appDbContext.Posts.FirstOrDefault(p => p.PostId == postId && loggedInUesrId == p.UserId);

        if (post != null){
            post.IsDeleted = true;
            _appDbContext.Posts.Update(post);
            await _appDbContext.SaveChangesAsync();

            // fix the hashtag implication 
            List<string> hashtagsOfPost = HashtagHelper.ExtractHashtags(post.Content);

            if (hashtagsOfPost != new List<string>())
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

}
