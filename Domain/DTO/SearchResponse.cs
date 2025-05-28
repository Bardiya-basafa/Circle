namespace Domain.DTO;

using Entities;


public class SearchResponse {

    public List<User> Users { get; set; }

    public List<Post> Posts { get; set; }

    public List<User> UserFriends { get; set; }

}
