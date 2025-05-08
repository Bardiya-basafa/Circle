namespace Domain.Entities;

using System.ComponentModel.DataAnnotations;


public class Story {

    [Key]
    public int StoryId { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsPrivate { get; set; } = false;

    public DateTime DateCreated { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Foreign Key 
    public int UserId { get; set; }

    // Navigation properties 
    public User User { get; set; }

    public ICollection<Like> Likes { get; set; } = new HashSet<Like>();

}
