using TaskFlow.Business.Domain;
using TaskFlow.Business.ViewModels;
using TaskFlow.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TaskFlow.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Policy = "AdminOnly")]
    public class UserRoleController : Controller
    {
        private readonly UserRoleDomain _userRoleDomain;

        public UserRoleController(UserRoleDomain userRoleDomain)
        {
            _userRoleDomain = userRoleDomain;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _userRoleDomain.GetListAsync();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserRoleCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoleName,RoleNameAr")] UserRoleCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _userRoleDomain.InsertAsync(vm, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.RoleInsertSuccess;
                    return RedirectToAction(nameof(Index));
                }
                if (result == 0 || result == 2)
                {
                    ModelState.AddModelError("RoleName", DashboardResource.RoleNameExists);
                    return View(vm);
                }

                TempData["Failed"] = DashboardResource.RoleInsertFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.RoleInsertFailed;
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var vm = await _userRoleDomain.GetForEditAsync(id);
            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Guid,RoleName,RoleNameAr,IsActive")] UserRoleEditVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _userRoleDomain.UpdateAsync(vm, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.RoleUpdateSuccess;
                    return RedirectToAction(nameof(Index));
                }
                if (result == 0 || result == 2)
                {
                    ModelState.AddModelError("RoleName", DashboardResource.RoleNameExists);
                    return View(vm);
                }

                TempData["Failed"] = DashboardResource.RoleUpdateFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.RoleUpdateFailed;
            }

            return View(vm);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _userRoleDomain.DeleteAsync(id, userId);
                if (result == 1)
                    TempData["ok"] = DashboardResource.RoleDeleteSuccess;
                else if (result == -2)
                    TempData["Failed"] = DashboardResource.InUse;
                else
                    TempData["Failed"] = DashboardResource.RoleDeleteFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.RoleDeleteFailed;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}