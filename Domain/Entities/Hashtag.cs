namespace Domain.Entities;

public class Hashtag {

    public int Id { get; set; }

    public string Name { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public int Count { get; set; }

    // properties 
    public ICollection<Post> Posts { get; set; } = new HashSet<Post>();

}
