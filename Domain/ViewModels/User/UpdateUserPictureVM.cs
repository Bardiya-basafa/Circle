namespace Domain.ViewModels.User;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


public class UpdateUserPictureVm {

    [Required]
    public IFormFile? ProfilePictureImage { get; set; }

}
