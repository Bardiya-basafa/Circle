namespace Domain.ViewModels.Authentication;

using System.ComponentModel.DataAnnotations;


public class LoginVm {

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

}
