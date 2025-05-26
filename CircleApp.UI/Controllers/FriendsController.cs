using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.Controllers;

using Base;
using Domain.DTO;
using Domain.Entities;
using Infrastructure.Persistence.Helpers.Constansts;
using Microsoft.AspNetCore.Authorization;
using Services.Interfaces;


public class FriendsController : BaseController {

    private readonly IFriendsService _friendsService;

    public FriendsController(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    public async Task<IActionResult> GetFriendships()
    {
        int userId = GetUserId();
        var friends = await _friendsService.GetFriendships(userId);

        return View(friends);
    }

    public async Task<IActionResult> GetSentFriendshipRequests()
    {
        int userId = GetUserId();
        var pendingRequests = await _friendsService.GetSentFriendshipRequests(userId);


        return View(pendingRequests == null ? new List<SentRequestResponse>() : pendingRequests);
    }

    public async Task<IActionResult> GetReceivedFriendshipRequests()
    {
        int userId = GetUserId();
        var pendingRequests = await _friendsService.GetReceivedFriendshipRequests(userId);

        return View(pendingRequests);
    }

    [HttpPost]
    public async Task<IActionResult> AcceptFriendshipRequest(int senderId)
    {
        int userId = GetUserId();
        await _friendsService.UpdateFriendshipRequestStatus(userId, senderId, FriendShipStatus.Accepted);

        return RedirectToAction("GetReceivedFriendshipRequests");
    }

    [HttpPost]
    public async Task<IActionResult> CancelFriendshipRequest(int receiverId)
    {
        int userId = GetUserId();
        await _friendsService.CancelFriendshipRequest(userId, receiverId);

        return RedirectToAction("GetSentFriendshipRequests");
    }

    public async Task<IActionResult> SendFriendshipRequest(int receiverId)
    {
        int userId = GetUserId();
        await _friendsService.SendFriendshipRequest(userId, receiverId);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFriend(int friendshipId)
    {
        int userId = GetUserId();
        await _friendsService.RemoveFriendship(friendshipId);

        return RedirectToAction("GetFriendships");
    }

    [HttpPost]
    public async Task<IActionResult> OkButton(int friendshipRequestId)
    {
        await _friendsService.OkButton(friendshipRequestId);

        return RedirectToAction("GetSentFriendshipRequests");
    }

    [HttpPost]
    public async Task<IActionResult> RejectFriendshipRequest(int senderId)
    {
        int userId = GetUserId();
        await _friendsService.UpdateFriendshipRequestStatus(userId, senderId, FriendShipStatus.Rejected);

        return RedirectToAction("GetReceivedFriendshipRequests");
    }

}
