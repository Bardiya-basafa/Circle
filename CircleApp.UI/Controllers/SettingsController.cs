namespace CircleApp.UI.Controllers;

using Domain.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

[Authorize]
public class SettingsController : Controller {

    private readonly IUserService _userService;

    public SettingsController(IUserService userService)
    {
        _userService = userService;
    }

    public int LoggedUserId { get; set; } = 1;

    public async Task<IActionResult> Index()
    {
        var loggedInUser = await _userService.GetUserDataAsync(LoggedUserId);

        return View(loggedInUser);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserProfilePicture(UpdateUserPictureVm updateUserPictureVm)
    {
        if (updateUserPictureVm.ProfilePictureImage == null) return RedirectToAction("Index");

        if (updateUserPictureVm.ProfilePictureImage.Length > 0){
            var rootFoder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            if (updateUserPictureVm.ProfilePictureImage.ContentType.Contains("image")){
                var rootImageFolderPath = Path.Combine(rootFoder, "profile-pictures");
                Directory.CreateDirectory(rootImageFolderPath);
                var fileName = Guid.NewGuid() + Path.GetExtension(updateUserPictureVm.ProfilePictureImage.FileName);
                var filePath = Path.Combine(rootImageFolderPath, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create)){
                    await updateUserPictureVm.ProfilePictureImage.CopyToAsync(stream);
                }

                var newPath = "/profile-pictures/" + fileName;
                await _userService.UpdateUserProfilePicture(LoggedUserId, newPath);
            }
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserData(UpdateProfileVm updateProfileVm) => RedirectToAction("Index");

    [HttpPost]
    public async Task<IActionResult> UpdateUserPassword(UpdatePasswordVm updatePasswordVm) => RedirectToAction("Index");

}
