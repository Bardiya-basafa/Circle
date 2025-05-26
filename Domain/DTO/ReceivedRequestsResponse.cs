namespace Domain.DTO;

public class ReceivedRequestsResponse {

    public int UserId { get; set; }

    public string FullName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public DateTime DateSent { get; set; }

}
