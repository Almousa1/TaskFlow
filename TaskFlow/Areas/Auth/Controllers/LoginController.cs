using TaskFlow.Business.Domain;
using TaskFlow.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskFlow.Areas.Auth.Controllers
{
    [Area("Auth")]
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SystemUserDomain _systemUserDomain;

        public LoginController(SystemUserDomain systemUserDomain)
        {
            _systemUserDomain = systemUserDomain;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string required = "", string denied = "")
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Dashboard" });
            }

            if (!string.IsNullOrEmpty(required))
                TempData["SwalCode"] = "LoginRequired";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel accountInfo)
        {
            if (!ModelState.IsValid) return View(accountInfo);

            try
            {
                var systemUser = await _systemUserDomain.GetByEmail(accountInfo.Email?.Trim());
                if (systemUser == null)
                {
                    ModelState.AddModelError(string.Empty, AuthResource.Login_InvalidCredentials);
                    return View(accountInfo);
                }

                if (!systemUser.IsActive)
                {
                    ModelState.AddModelError(string.Empty, AuthResource.Login_InvalidCredentials);
                    return View(accountInfo);
                }

                if (!_systemUserDomain.VerifyPassword(accountInfo.Password, systemUser.Password))
                {
                    ModelState.AddModelError(string.Empty, AuthResource.Login_InvalidCredentials);
                    return View(accountInfo);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, systemUser.guid.ToString()),
                    new Claim(ClaimTypes.Name, systemUser.Name),
                    new Claim(ClaimTypes.Role, systemUser.UserRole?.RoleName ?? ""),
                    new Claim("RoleId", systemUser.UserRoleId.ToString()),
                    new Claim("UserId", systemUser.Id.ToString()),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Dashboard", new { area = "Dashboard" });
            }
            catch
            {
                ModelState.AddModelError(string.Empty, AuthResource.Login_UnexpectedError);
                return View(accountInfo);
            }
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["SwalCode"] = "LoggedOut";
            }
            return RedirectToAction("Index", "Login", new { area = "Auth" });
        }
    }
}
