namespace Domain.ViewModels.Users;

using DTO;
using Entities;


public class UserDetailsVm {

    public string FullName { get; set; }

    public string ProfilePictureUrl { get; set; }

    public string UserName { get; set; }

    public List<Post> Posts { get; set; }

    public List<FriendResponse> Friends { get; set; }

}
