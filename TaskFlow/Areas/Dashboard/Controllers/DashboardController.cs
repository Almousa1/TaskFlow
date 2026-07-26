using TaskFlow.Business.Domain;
using TaskFlow.Business.ViewModels;
using TaskFlow.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskFlow.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly TodoItemDomain _todoItemDomain;

        public DashboardController(TodoItemDomain todoItemDomain)
        {
            _todoItemDomain = todoItemDomain;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue("UserId");
            int.TryParse(userIdStr, out var userId);
            var vm = await _todoItemDomain.GetStats(userId);
            return View(vm);
        }
    }
}
