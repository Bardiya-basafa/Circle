namespace Domain.ViewModels.Authentication;

using System.ComponentModel.DataAnnotations;


public class RegisterVm {

    [Required(ErrorMessage = "first name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "first name must be between 2 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "last name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "last name must be between 2 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "email is required")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "password is required")]
    public string Password { get; set; }

    [Required(ErrorMessage = "confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }

}
