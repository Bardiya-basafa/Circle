namespace Domain.Entities;

public class Comment {

    public int Id { get; set; }

    public string Content { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    // foriegn key 
    public int UserId { get; set; }

    public int PostId { get; set; }

    // navigation properties 
    public User User { get; set; }

    public Post Post { get; set; }

}
