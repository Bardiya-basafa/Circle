namespace CircleApp.UI.Controllers;

using System.Security.Claims;
using Domain.Entities;
using Domain.ViewModels.Authentication;
using Domain.ViewModels.User;
using Infrastructure.Persistence.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


public class AuthenticationController : Controller {

    private readonly SignInManager<User> _signInManager;

    private readonly UserManager<User> _userManager;

    public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET
    public async Task<IActionResult> Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm loginVm)
    {
        if (!ModelState.IsValid){
            return View(loginVm);
        }

        var user = await _userManager.FindByEmailAsync(loginVm.Email);

        if (user == null){
            ModelState.AddModelError(string.Empty, "Invalid username or password.");

            return View(loginVm);
        }

        var userClaims = await _userManager.GetClaimsAsync(user);

        if (userClaims.Any(c => c.Type == "FullName")){
            await _userManager.AddClaimAsync(user, new Claim("FullName", user.FullName));
        }

        var result = await _signInManager.PasswordSignInAsync(loginVm.Email, loginVm.Password, false, false);

        if (result.Succeeded){
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt");

        return View(loginVm);
    }

    public async Task<IActionResult> Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm registerVm)
    {
        if (!ModelState.IsValid){
            return View(registerVm);
        }

        var newUser = new User()
        {
            FullName = $"{registerVm.FirstName} {registerVm.LastName}",
            Email = registerVm.Email,
            UserName = registerVm.Email
        };

        var User = await _userManager.FindByEmailAsync(registerVm.Email);

        if (User != null){
            ModelState.AddModelError("Email", $"{User.Email} already exists");

            return View(registerVm);
        }

        var result = await _userManager.CreateAsync(newUser, registerVm.Password);

        if (result.Succeeded){
            await _userManager.AddToRoleAsync(newUser, AppRoles.User);
            await _userManager.AddClaimAsync(newUser, new Claim("FullName", $"{registerVm.FirstName} {registerVm.LastName}"));


            await _signInManager.SignInAsync(newUser, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }
        else{
            foreach (var error in result.Errors){
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }


        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateUserPassword(UpdatePasswordVm updatePasswordVm)
    {
        if (!ModelState.IsValid){
            TempData["PasswordError"] = "Invalid attempt to update password.";
            TempData["ActiveTab"] = "Password";

            return RedirectToAction("Index", "Settings");
        }

        if (updatePasswordVm.NewPassword != updatePasswordVm.ConfirmPassword){
            TempData["PasswordCompare"] = "Passwords do not match.";
            TempData["ActiveTab"] = "Password";

            return RedirectToAction("Index", "Settings");
        }

        var loggedInUser = await _userManager.GetUserAsync(User);
        var isCusrrentPasswordValid = await _userManager.CheckPasswordAsync(loggedInUser, updatePasswordVm.CurrentPassword);

        if (!isCusrrentPasswordValid){
            TempData["IsPasswordValid"] = "Current password is invalid.";
            TempData["ActiveTab"] = "Password";

            return RedirectToAction("Index", "Settings");
        }

        var result = await _userManager.ChangePasswordAsync(loggedInUser, updatePasswordVm.CurrentPassword, updatePasswordVm.NewPassword);

        if (result.Succeeded){
            TempData["UpdatedPassword"] = "Password updated successfully.";
            TempData["ActiveTab"] = "Password";
            await _signInManager.RefreshSignInAsync(loggedInUser);
        }
        else{
            string errorMessages = string.Join("<br/>", result.Errors.Select(e => e.Description));
            TempData["NewPasswordErrors"] = errorMessages;
            TempData["ActiveTab"] = "Password";
        }

        return RedirectToAction("Index", "Settings");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateUserData(UpdateProfileVm updateProfileVm)
    {
        if (!ModelState.IsValid){
            TempData["ProfileError"] = "Invalid attempt to update profile.";
            TempData["ActiveTab"] = "Profile";

            return RedirectToAction("Index", "Settings");
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null){
            return RedirectToAction("Login");
        }

        user.FullName = updateProfileVm.FullName;

        if (updateProfileVm.Bio != null){
            user.Bio = updateProfileVm.Bio;
            TempData["Bio"] = user.Bio;
        }


        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded){
            string errorMessages = string.Join("<br/>", result.Errors.Select(e => e.Description));
            TempData["UpdateProfileErrors"] = errorMessages;
            TempData["ActiveTab"] = "Profile";

            return RedirectToAction("Index", "Settings");
        }
        else{
            TempData["UpdatedProfile"] = "Profile successfully.";
            TempData["ActiveTab"] = "Profile";
        }

        return RedirectToAction("Index", "Settings");
    }


    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }

}
