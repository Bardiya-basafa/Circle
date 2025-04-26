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
            .OrderByDescending(n => n.DateCreated)
            .ToListAsync();

        return View(allPosts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStatus(PostVM post)
    {
        var newPost = new Post
        {
            Content = post.Conent,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = "",
            UserId = 1,
            NrOfReports = 0
        };

        if (post.Image != null && post.Image.Length > 0){
            var rootFoder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (post.Image.ContentType.Contains("image")){
                var rootImageFolderPath = Path.Combine(rootFoder, "images");
                Directory.CreateDirectory(rootImageFolderPath);
                var fileName = Guid.NewGuid() + Path.GetExtension(post.Image.FileName);
                var filePath = Path.Combine(rootImageFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create)){
                    await post.Image.CopyToAsync(stream);
                }

                newPost.ImageUrl = "/images/" + fileName;
            }
        }

        return RedirectToAction("Index");
    }

}
