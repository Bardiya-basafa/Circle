namespace Domain.Entities;

public class Bookmark {

    public int Id { get; set; }

    // foreign key
    public int UserId { get; set; }

    public int PostId { get; set; }

    // navigation properties
    public Post Post { get; set; }

    public User User { get; set; }

}
