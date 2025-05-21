namespace Domain.ViewModels.User;

using System.ComponentModel.DataAnnotations;


public class UpdatePasswordVm {

    [Required(ErrorMessage = "Password is required")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "NewPassword is required")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "ConfirmNewPassword is required")]
    public string ConfirmPassword { get; set; }

}
