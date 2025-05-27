namespace CircleApp.UI.Views.Shared;



public static class ViewHelper {

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

}
