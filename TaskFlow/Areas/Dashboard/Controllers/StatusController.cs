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
    public class StatusController : Controller
    {
        private readonly StatusDomain _statusDomain;

        public StatusController(StatusDomain statusDomain)
        {
            _statusDomain = statusDomain;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _statusDomain.GetStatusListItems();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new StatusCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatusName,StatusNameAr")] StatusCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _statusDomain.InsertStatus(vm, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.StatusInsertSuccess;
                    return RedirectToAction(nameof(Index));
                }
                if (result == -2)
                {
                    ModelState.AddModelError("StatusName", DashboardResource.StatusAlreadyExists);
                    return View(vm);
                }

                TempData["Failed"] = DashboardResource.StatusInsertFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.StatusInsertFailed;
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var vm = await _statusDomain.GetStatusEditViewModel(id);
            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Guid,StatusName,StatusNameAr,Approved")] StatusEditVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _statusDomain.UpdateStatus(vm, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.StatusUpdateSuccess;
                    return RedirectToAction(nameof(Index));
                }
                if (result == -2)
                {
                    ModelState.AddModelError("StatusName", DashboardResource.StatusAlreadyExists);
                    return View(vm);
                }

                TempData["Failed"] = DashboardResource.StatusUpdateFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.StatusUpdateFailed;
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
                var result = await _statusDomain.DeleteStatus(id, userId);
                if (result == 1)
                    TempData["ok"] = DashboardResource.StatusDeleteSuccess;
                else if (result == -3)
                    TempData["Failed"] = DashboardResource.StatusInUseCannotDelete;
                else
                    TempData["Failed"] = DashboardResource.StatusDeleteFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.StatusDeleteFailed;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}