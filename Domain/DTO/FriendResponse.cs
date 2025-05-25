namespace Domain.DTO;

public class FriendResponse {

    public int FriendId { get; set; }
    public string FullName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public DateTime CreatedDate { get; set; }

}
