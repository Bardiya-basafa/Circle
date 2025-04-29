namespace Domain.Entities;

using System.ComponentModel.DataAnnotations;


public class Post {

    [Key]
    public int PostId { get; set; }

    public string Content { get; set; }

    public string? ImageUrl { get; set; }

    public int NrOfReports { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    // Foregin Key 
    public int UserId { get; set; }

    // Navigation properties 
    public User User { get; set; }

    public ICollection<Like> Likes { get; set; } = new HashSet<Like>();

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    public ICollection<Bookmark> Bookmarks { get; set; } = new HashSet<Bookmark>();

}
