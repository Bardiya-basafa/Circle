using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.Controllers;

using Infrastructure.Persistence.Constants;
using Microsoft.AspNetCore.Authorization;
using Services.Interfaces;


[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller {

    private readonly IPostService _postService;

    private readonly IAdminService _adminService;

    public AdminController(IPostService postService, IAdminService adminService)
    {
        _postService = postService;
        _adminService = adminService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var reportedPosts = await _postService.GetReportedPosts();


        return View(reportedPosts);
    }

    public async Task<IActionResult> DismissPostReports(int postId)
    {
        await _postService.DismissPostReports(postId);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> RemoveReportedPost(int postId)
    {
        await _postService.RemoveReportedPost(postId);

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> PostReports(int postId)
    {
        var reports = await _adminService.GetPostReports(postId);

        return View(reports);
    }

}
