using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarePlusDesktop.Controllers;

public abstract class BaseAdminController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetString("AdminUser") == null)
        {
            context.Result = RedirectToAction("Login", "Account");
            return;
        }
        base.OnActionExecuting(context);
    }
}
