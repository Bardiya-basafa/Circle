namespace CircleApp.UI.Controllers;

using System.Security.Claims;
using Domain.Entities;
using Domain.ViewModels.Authentication;
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
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }

}
