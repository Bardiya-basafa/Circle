namespace Domain.Entities;

public class Report {

    //key
    public int Id { get; set; }

    public DateTime DateCreated { get; set; }

    // foreign keys
    public int UserId { get; set; }

    public int PostId { get; set; }

    // navigation properties
    public User User { get; set; }

    public Post Post { get; set; }

}
