using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.Controllers;

using Base;
using Domain.Entities;
using Infrastructure.Persistence.Constants;
using Microsoft.AspNetCore.Authorization;
using Services.Interfaces;


[Authorize]
public class NotificationsController : BaseController {

    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetNotificationsCount()
    {
        int userId = GetUserId();
        var notificationsCount = await _notificationService.GetNewNotificationsCount(userId);

        return Json(notificationsCount);
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        int userId = GetUserId();
        var allNotifications = await _notificationService.GetAllNotifications(userId);

        return PartialView("Notifications/_Notifications", allNotifications);
    }

    [HttpPost]
    public async Task<IActionResult> SetNotificationAsRead(int notificationId)
    {
        int userId = GetUserId();
        await _notificationService.SetNotificationAsRead(notificationId);
        var allNotifications = await _notificationService.GetAllNotifications(userId);

        return PartialView("Notifications/_Notifications", allNotifications);
    }

    public async Task<IActionResult> RedirectNotification(string notificationType)
    {
        switch (notificationType){
            case NotificationTypes.Comment: return RedirectToAction("Index", "Home"); break;

            case NotificationTypes.Like: return RedirectToAction("Index", "Home"); break;

            case NotificationTypes.Bookmark: return RedirectToAction("Index", "Home"); break;

            case NotificationTypes.FriendRequestAccepted: return RedirectToAction("GetFriendships", "Friends"); break;

            case NotificationTypes.FriendRequest: return RedirectToAction("GetSentFriendshipRequests", "Friends"); break;

            default: return RedirectToAction("Index", "Home"); break;
        }
    }

}
