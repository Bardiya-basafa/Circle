namespace CircleApp.UI.Controllers;

using Domain.Entities;
using Domain.ViewModels.Home;
using Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


public class HomeController : Controller {

    private readonly ILogger<HomeController> _logger;

    private readonly AppDbContext _appDbContext;

    public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbContext = appDbContext;
    }

    public async Task<IActionResult> Index()
    {
        var allPosts = await _appDbContext.Posts
            .Include(n => n.User)
            .OrderByDescending(n => n.DateCreated)
            .ToListAsync();

        return View(allPosts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStatus(PostVM post)
    {
        Post newPost = new Post()
        {
            Content = post.Conent,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = "",
            UserId = 1,
            NrOfReports = 0,
        };

        if (post.Image != null && post.Image.Length > 0){
            string rootFoder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (post.Image.ContentType.Contains("image")){
                string rootImageFolderPath = Path.Combine(rootFoder, "images");
                Directory.CreateDirectory(rootImageFolderPath);
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                string filePath = Path.Combine(rootImageFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create)){
                    await post.Image.CopyToAsync(stream);
                }
                newPost.ImageUrl = "/images/" + fileName;
            }
        }

        return RedirectToAction("Index");
    }

}
