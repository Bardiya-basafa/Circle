using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.Controllers;

using Base;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Services.Interfaces;


public class FriendsController : BaseController {

    private readonly IFriendsService _friendsService;

    public FriendsController(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    public async Task<IActionResult> Friends()
    {
        int userId = GetUserId();
        var friends = await _friendsService.GetFriends(userId);

        return View(friends);
    }

    public async Task<IActionResult> FriendRequests()
    {
        int userId = GetUserId();
        var pendingRequests = await _friendsService.GetSentRequests(userId);


        return View(pendingRequests == null ? new List<SentRequestResponse>() : pendingRequests);
    }

    [HttpPost]
    public async Task<IActionResult> CancelFriendRequest(int receiverId)
    {
        int userId = GetUserId();
        await _friendsService.CancelFriendRequest(userId, receiverId);

        // return RedirectToAction("SentRequests");
        return RedirectToAction("FriendRequests");
    }

    public async Task<IActionResult> SendFriendRequest(int receiverId)
    {
        int userId = GetUserId();
        await _friendsService.SendFriendshipRequest(userId, receiverId);

        return RedirectToAction("Index", "Home");
    }

}
