namespace Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Post {
    [Key]
    public int PostID { get; set; }
    public string Content { get; set; }
    public string? ImageURL { get; set; }
    public int NrOfReports { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateUpdated { get; set; }
}
