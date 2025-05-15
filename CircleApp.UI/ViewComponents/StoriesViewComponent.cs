namespace CircleApp.UI.ViewComponents;

using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class StoriesViewComponent : ViewComponent {

    private readonly AppDbContext _appDbContext;

    public StoriesViewComponent(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public int LoggedInUserId { get; set; } = 1;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<Story>? allStories = await _appDbContext.Stories
            .Where(s => !s.IsDeleted && (s.UserId == LoggedInUserId || !s.IsPrivate))
            .Include(s => s.User)
            .Include(s => s.Likes)
            .OrderByDescending(s => s.DateCreated)
            .ToListAsync();

        return View(allStories);
    }

}
