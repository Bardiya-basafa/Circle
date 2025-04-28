namespace Domain.Entities;

using System.ComponentModel.DataAnnotations;


public class User {

    [Key]
    public int UserId { get; set; }

    public string FullName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    // navigation properties 
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();

}
