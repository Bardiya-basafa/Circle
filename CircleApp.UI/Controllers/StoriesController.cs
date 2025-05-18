namespace CircleApp.UI.Controllers;

using Domain.Entities;
using Domain.ViewModels.Stroy;
using Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class StoriesController : Controller {

    private readonly AppDbContext _appDbContext;

    private readonly ILogger _logger;

    public StoriesController(AppDbContext appDbContext, ILogger<StoriesController> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public int LoggedInUserId { get; set; } = 1;

    public async Task<IActionResult> Index()
    {
        List<Story> allStories = await _appDbContext.Stories
            .Where(s => !s.IsDeleted && (s.UserId == LoggedInUserId || !s.IsPrivate) && s.DateCreated >= DateTime.UtcNow.AddHours(-24))
            .Include(s => s.User)
            .Include(s => s.Likes)
            .OrderByDescending(s => s.DateCreated)
            .ToListAsync();

        return View(allStories);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStory(CreateStoryVm story)
    {
        if (story.Image == null) return RedirectToAction("Index", "Home");

        if (story.Image.Length > 0){
            var newStory = new Story
            {
                DateCreated = DateTime.UtcNow,
                UserId = LoggedInUserId
            };

            var rootFoder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (story.Image.ContentType.Contains("image")){
                var rootImageFolderPath = Path.Combine(rootFoder, "story-images");
                Directory.CreateDirectory(rootImageFolderPath);
                var fileName = Guid.NewGuid() + Path.GetExtension(story.Image.FileName);
                var filePath = Path.Combine(rootImageFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create)){
                    await story.Image.CopyToAsync(stream);
                }

                newStory.ImageUrl = "/story-images/" + fileName;
            }

            _appDbContext.Stories.Add(newStory);
            await _appDbContext.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home");
    }

}
