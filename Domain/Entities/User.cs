namespace Domain.Entities;

using System.ComponentModel.DataAnnotations;


public class User {

    [Key]
    public int UserId { get; set; }

    public string FullName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    // user posts 
    public ICollection<Post> Posts { get; set; } = new List<Post>();

}
