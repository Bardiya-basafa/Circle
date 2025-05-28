namespace CircleApp.UI.Views.Shared;

using Infrastructure.Persistence.Constants;


public static class ViewHelper {

    public static bool ShowCemments { get; set; } = false;

    public static string DisplayTime(DateTime dateTime)
    {
        var timeSincePostCreated = DateTime.UtcNow.Subtract(dateTime);
        var displayTime = "";

        if (timeSincePostCreated.TotalDays >= 1){
            displayTime = $"{timeSincePostCreated.Days} days ago";
        }
        else if (timeSincePostCreated.Hours >= 1){
            displayTime = $"{timeSincePostCreated.Hours} hours ago";
        }
        else if (timeSincePostCreated.Minutes >= 1){
            displayTime = $"{timeSincePostCreated.Minutes} minutes ago";
        }
        else if (timeSincePostCreated.Seconds >= 1){
            displayTime = "just now";
        }

        return displayTime;
    }

    public static string GetNotificationIcon(string notificationType)
    {
        var notificationIcon = "";

        switch (notificationType){
            case NotificationTypes.Like: notificationIcon = "heart-outline"; break;

            case NotificationTypes.Comment: notificationIcon = "chatbubbles"; break;

            case NotificationTypes.Bookmark: notificationIcon = "bookmark-outline"; break;

            case NotificationTypes.FriendRequest: notificationIcon = "person-add-outline"; break;

            case NotificationTypes.FriendRequestAccepted: notificationIcon = "person-outline"; break;

            default: notificationIcon = "notification-outline"; break;
        }

        return notificationIcon;
    }

}
