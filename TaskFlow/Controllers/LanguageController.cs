using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace TaskFlow.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult Set(string culture, string returnUrl = "/")
        {
            culture = (culture == "en") ? "en" : "ar";
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = false,
                    HttpOnly = false,
                    Path = "/"
                });
            return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
        }
    }
}