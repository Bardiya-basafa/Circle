using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.ViewComponents;

using Domain.Entities;
using Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;


public class StoriesViewComponent : ViewComponent {

    private readonly AppDbContext _appDbContext;

    public int LoggedInUserId { get; set; } = 1;

    public StoriesViewComponent(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var allStories = _appDbContext.Stories
            .Where(s => !s.IsDeleted && (s.UserId == LoggedInUserId || !s.IsPrivate))
            .Include(s => s.User)
            .Include(s => s.Likes)
            .OrderByDescending(s => s.DateCreated)
            .ToList();

        return View(allStories);
    }

}
