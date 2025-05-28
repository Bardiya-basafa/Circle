namespace Domain.ViewModels.Home;

using Entities;


public class PartialViewPostVm {

    public Post Post { get; set; }

    public bool? ShowDetails { get; set; }

    public bool? ShowAllComments { get; set; }

}
