namespace Domain.Entities;

public class FriendRequest {

    public int Id { get; set; }

    public DateTime DateCreated { get; set; }

    public string Status { get; set; }

    public int SenderId { get; set; }

    public User Sender { get; set; }

    public int ReceiverId { get; set; }

    public User Receiver { get; set; }

}
