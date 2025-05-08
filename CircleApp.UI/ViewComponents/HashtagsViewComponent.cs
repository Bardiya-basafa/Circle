namespace CircleApp.UI.ViewComponents;

using Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class HashtagsViewComponent : ViewComponent {

    private readonly AppDbContext _appDbContext;

    public HashtagsViewComponent(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var allHashtags = await _appDbContext.Hashtags
            .Where(h => h.DateUpdated >= DateTime.UtcNow.AddDays(-7))
            .OrderByDescending(h => h.Count)
            .Take(3)
            .ToListAsync();

        return View(allHashtags);
    }

}
