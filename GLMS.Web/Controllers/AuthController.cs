using GLMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthApiService _auth;
        private readonly IConfiguration _config;

        public AuthController(
            AuthApiService auth,
            IConfiguration config)
        {
            _auth = auth;
            _config = config;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(
            string username,
            string password)
        {
            var baseUrl = _config["ApiSettings:BaseUrl"];

            var result = await _auth.Login(
                username,
                password,
                baseUrl);

            if (result == null)
            {
                ViewBag.Error = "Invalid login";
                return View();
            }

            HttpContext.Session.SetString(
                SessionKeys.User,
                result.Username);

            HttpContext.Session.SetString(
                SessionKeys.Role,
                result.Role);

            return RedirectToAction(
                "Index",
                "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login");
        }
    }
}