namespace CircleApp.UI.ViewComponents;

using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class HashtagsViewComponent : ViewComponent {

    private readonly AppDbContext _appDbContext;

    public HashtagsViewComponent(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var posts = ViewData["posts"] as List<Post>;
        var postIds = posts.Select(p => p.PostId).ToList();
        var oneWeekAgoNow = DateTime.UtcNow.AddDays(-7);

        var top3Hashtags = await _appDbContext.Hashtags
            .Where(h => h.DateCreated >= oneWeekAgoNow && h.Posts.Any(p => postIds.Contains(p.PostId)) && h.Count > 0)
            .OrderByDescending(n => n.Count)
            .Distinct()
            .Take(3)
            .ToListAsync();


        return View(top3Hashtags);
    }

}
