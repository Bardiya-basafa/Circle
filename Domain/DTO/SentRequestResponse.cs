namespace Domain.DTO;

public class SentRequestResponse {

    public int SentRequestUserId { get; set; }

    public string FullName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public DateTime CreatedDate { get; set; }

}
