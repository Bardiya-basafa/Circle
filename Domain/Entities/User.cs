namespace Domain.Entities;

using System.ComponentModel.DataAnnotations;


public class User {

    [Key]
    public int UserId { get; set; }

    public string FullName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    // navigation properties 
    public ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    public ICollection<Like> Likes { get; set; } = new HashSet<Like>();

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    public ICollection<Bookmark> Bookmarks { get; set; } = new HashSet<Bookmark>();

    public ICollection<Report> Reports { get; set; } = new HashSet<Report>();

}
