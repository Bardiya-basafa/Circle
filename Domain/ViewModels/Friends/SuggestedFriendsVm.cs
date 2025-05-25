namespace Domain.ViewModels.Friends;

public class SuggestedFriendsVm {

    public int UserId { get; set; }
    public string? ProfilePtctureUrl { get; set; }

    public string? FullName { get; set; }

    public int FriendsCount { get; set; }

    public string ShowFollowers => FriendsCount == 0 ? "No followers" : FriendsCount == 1 ? "1 Follower" : $"{FriendsCount} Followers";

}
