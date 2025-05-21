namespace Domain.ViewModels.User;

using System.ComponentModel.DataAnnotations;


public class UpdateProfileVm {

    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; }

    public string Bio { get; set; }

}
