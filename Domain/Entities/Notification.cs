namespace Domain.Entities;

public class Notification {

    public int Id { get; set; }

    public int UserId { get; set; }

    public bool IsRead { get; set; }

    public string Content { get; set; }

    public string Type { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateModified { get; set; }

}
