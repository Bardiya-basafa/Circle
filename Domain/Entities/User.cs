namespace Domain.Entities;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


public class User : IdentityUser<int> {


    public string FullName { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public bool IsDeleted { get; set; } = false;

    // navigation properties 
    public ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    public ICollection<Story> Stories { get; set; } = new HashSet<Story>();

    public ICollection<Like> Likes { get; set; } = new HashSet<Like>();

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    public ICollection<Bookmark> Bookmarks { get; set; } = new HashSet<Bookmark>();

    public ICollection<Report> Reports { get; set; } = new HashSet<Report>();

}
