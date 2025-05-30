namespace Services.Services;

using Domain.Entities;
using Hubs;
using Infrastructure.Persistence.Constants;
using Infrastructure.Persistence.DbContexts;
using Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


public class NotificationService : INotificationService {

    private readonly AppDbContext _context;

    private readonly IHubContext<NotificationsHub> _hubContext;


    public NotificationService(AppDbContext context, IHubContext<NotificationsHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task AddNewNotification(int userId, string notificationType, string userFullName)
    {
        var newNotification = new Notification()
        {
            UserId = userId,
            Type = notificationType,
            Content = GetNotificationMessage(userFullName, notificationType),
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow,
            IsRead = false
        };

        await _context.Notifications.AddAsync(newNotification);
        await _context.SaveChangesAsync();
        var notificationsCount = await GetNewNotificationsCount(userId);

        await _hubContext.Clients.User(userId.ToString())
            .SendAsync("ReceiveNotification", notificationsCount);
    }


    public async Task<int> GetNewNotificationsCount(int userId)
    {
        var notificationsCount = await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).CountAsync();

        return notificationsCount;
    }


    public async Task<List<Notification>> GetAllNotifications(int userId)
    {
        var allNotifications = await _context.Notifications.Where(n => n.UserId == userId)
            .OrderBy(n => n.IsRead)
            .ThenByDescending(n => n.DateCreated)
            .ToListAsync();

        return allNotifications;
    }

    public async Task SetNotificationAsRead(int notificationId)
    {
        var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

        if (notification != null){
            notification.IsRead = true;
            _context.Update(notification);
            await _context.SaveChangesAsync();
        }
    }

    private string GetNotificationMessage(string userFullName, string notificationType)
    {
        var message = "";

        switch (notificationType){
            case NotificationTypes.Like:

                message = $"{userFullName} Liked your post";

                break;

            case NotificationTypes.Comment:

                message = $"{userFullName} commented your post";

                break;

            case NotificationTypes.FriendRequest:

                message = $"{userFullName} sent a friend request";

                break;

            case NotificationTypes.FriendRequestAccepted:

                message = $"{userFullName} accepted your friend request";

                break;

            case NotificationTypes.Post:

                message = "One of you post has been removed because of reports";

                break;

            default:

                break;
        }

        return message;
    }

}
