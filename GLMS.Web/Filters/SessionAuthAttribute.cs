using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GLMS.Web.Filters
{
    public class SessionAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(
            ActionExecutingContext context)
        {
            var user =
                context.HttpContext.Session.GetString(
                    SessionKeys.User);

            if (string.IsNullOrEmpty(user))
            {
                context.Result =
                    new RedirectToActionResult(
                        "Login",
                        "Auth",
                        null);
            }
        }
    }
}