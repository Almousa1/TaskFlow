using TaskFlow.Business.Domain;
using TaskFlow.Business.ViewModels;
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
    public class SignUpController : Controller
    {
        private readonly SystemUserDomain _systemUserDomain;
        private readonly UserRoleDomain _userRoleDomain;

        public SignUpController(SystemUserDomain systemUserDomain, UserRoleDomain userRoleDomain)
        {
            _systemUserDomain = systemUserDomain;
            _userRoleDomain = userRoleDomain;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard", new { area = "Dashboard" });

            return View(new SignUpViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SignUpViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var existing = await _systemUserDomain.GetByEmail(vm.Email.Trim());
                if (existing != null)
                {
                    ModelState.AddModelError("Email", AuthResource.SignUp_EmailExists);
                    return View(vm);
                }

                var role = await _userRoleDomain._UserRoleRepository.FindByNameAsync("User");
                if (role == null)
                {
                    var roles = await _userRoleDomain._UserRoleRepository.GetAllActiveAsync();
                    role = roles.FirstOrDefault();
                }

                if (role == null)
                {
                    ModelState.AddModelError(string.Empty, AuthResource.SignUp_NoRole);
                    return View(vm);
                }

                var createVm = new UserCreateVM
                {
                    Email = vm.Email.Trim(),
                    Name = vm.Name,
                    NameAr = vm.NameAr,
                    Password = vm.Password,
                    UserRoleId = role.Id
                };

                var result = await _systemUserDomain.InsertUser(createVm, 0);
                if (result != 1)
                {
                    ModelState.AddModelError(string.Empty, AuthResource.SignUp_Failed);
                    return View(vm);
                }

                var user = await _systemUserDomain.GetByEmail(vm.Email.Trim());

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.guid.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, role.RoleName),
                    new Claim("RoleId", user.UserRoleId.ToString()),
                    new Claim("UserId", user.Id.ToString()),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["ok"] = AuthResource.SignUp_Success;
                return RedirectToAction("Index", "Dashboard", new { area = "Dashboard" });
            }
            catch
            {
                ModelState.AddModelError(string.Empty, AuthResource.Login_UnexpectedError);
                return View(vm);
            }
        }
    }
}