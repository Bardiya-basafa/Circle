using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.Controllers;

using Base;
using Domain.Entities;
using Domain.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;


[Authorize]
public class UsersController : BaseController {

    private readonly IUserService _userService;

    private readonly IFriendsService _friendsService;

    private readonly UserManager<User> _userManager;

    public UsersController(IUserService userService, UserManager<User> userManager, IFriendsService friendsService)
    {
        _userService = userService;
        _userManager = userManager;
        _friendsService = friendsService;
    }

    [HttpGet]

    // GET
    public async Task<IActionResult> Details(int userId)
    {
        var posts = await _userService.GetUserPostsByIdAsync(userId);
        var friends = await _friendsService.GetFriendships(userId);
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null){
            RedirectToAction("Index", "Home");
        }


        var details = new UserDetailsVm()
        {
            ProfilePictureUrl = user.ProfilePictureUrl,
            FullName = user.FullName,
            Posts = posts,
            Friends = friends
        };

        return View(details);
    }

}
