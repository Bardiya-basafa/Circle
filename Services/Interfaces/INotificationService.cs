namespace Services.Interfaces;

using Domain.Entities;


public interface INotificationService {

    Task AddNewNotification(int userId, string notificationType, string userFullName);

    Task<int> GetNewNotificationsCount(int userId);

    Task<List<Notification>> GetAllNotifications(int userId);

    Task SetNotificationAsRead(int notificationId);

}
