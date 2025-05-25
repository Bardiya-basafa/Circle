namespace Domain.Entities;

public class FriendShip {

    public int Id { get; set; }


    public DateTime DateCreated { get; set; }

    public DateTime DateAccepted { get; set; }

    // sender properties
    public int SenderId { get; set; }

    public User Sender { get; set; }

    // receiver properites
    public int ReceiverId { get; set; }

    public User Receiver { get; set; }

}
