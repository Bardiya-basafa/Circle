namespace CircleApp.UI.Controllers.Base;

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;


public abstract class BaseController : Controller {

    protected int GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int id;

        if (userId == null){
            RedirectToLogin();
        }

        if (int.TryParse(userId, out id)){
            return id;
        }

        RedirectToLogin();

        return 0;
    }

    protected string GetUserFullName()
    {
        var userFullName = User.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;

        return userFullName ?? "full name";
    }

    protected IActionResult RedirectToLogin()
    {
        return RedirectToAction("Login", "Authentication");
    }

}
