namespace CircleApp.UI.ViewComponents;

using System.Security.Claims;
using Controllers.Base;
using Domain.Entities;
using Domain.ViewModels.Friends;
using Infrastructure.Persistence.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;


[Authorize]
public class FriendsViewComponent : ViewComponent {

    private readonly IFriendsService _friendsService;


    public FriendsViewComponent(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var loggedInUserId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
        var suggestedFriends = await _friendsService.GetSuggestedFriends(int.Parse(loggedInUserId));

        var ViewModel =  suggestedFriends.Select(sf =>
                new SuggestedFriendsVm()
                {
                    UserId = sf.User.Id,
                    FullName = sf.User.FullName,
                    ProfilePtctureUrl = sf.User.ProfilePictureUrl,
                    FriendsCount = sf.FollowerCount,
                }
            )
            .ToList();

        return View(ViewModel);
    }

}
