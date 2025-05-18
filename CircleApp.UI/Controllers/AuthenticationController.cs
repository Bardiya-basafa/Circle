using Microsoft.AspNetCore.Mvc;


namespace CircleApp.UI.Controllers;

using Domain.Entities;
using Domain.ViewModels.Authentication;
using Infrastructure.Persistence.Constants;
using Microsoft.AspNetCore.Identity;


public class AuthenticationController : Controller {

    private readonly UserManager<User> _userManager;

    private readonly SignInManager<User> _signInManager;

    public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(AuthenticationVm loginUser)
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task Register(AuthenticationVm registerUser)
    {
        try{
            var newUser = new User()

            {
                Email = registerUser.Email,
                FullName = $"{registerUser.FirstName} {registerUser.LastName}",
                UserName = registerUser.Email
            };

            var result = await _userManager.CreateAsync(newUser, registerUser.Password);

            if (result.Succeeded){
                await _userManager.AddToRoleAsync(newUser, AppRoles.User);

                await _signInManager.SignInAsync(newUser, isPersistent: false);

                RedirectToAction("Index", "Home");
            }
        }
        catch (Exception e){
            Console.WriteLine(e);

            throw;
        }
    }

}
