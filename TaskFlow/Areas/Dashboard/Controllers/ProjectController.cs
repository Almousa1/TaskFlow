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
    public class ProjectController : Controller
    {
        private readonly ProjectDomain _projectDomain;

        public ProjectController(ProjectDomain projectDomain)
        {
            _projectDomain = projectDomain;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue("UserId");
            int.TryParse(userIdStr, out var userId);
            var model = await _projectDomain.GetProjectListItems(userId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ProjectCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Color")] ProjectCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _projectDomain.InsertProject(vm, userId, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.ProjectInsertSuccess;
                    return RedirectToAction(nameof(Index));
                }
                if (result == -2)
                {
                    ModelState.AddModelError("Name", DashboardResource.StatusAlreadyExists);
                    return View(vm);
                }

                TempData["Failed"] = DashboardResource.ProjectInsertFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.ProjectInsertFailed;
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var vm = await _projectDomain.GetProjectEditViewModel(id);
            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Guid,Name,Description,Color,IsActive")] ProjectEditVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _projectDomain.UpdateProject(vm, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.ProjectUpdateSuccess;
                    return RedirectToAction(nameof(Index));
                }
                if (result == -2)
                {
                    ModelState.AddModelError("Name", DashboardResource.StatusAlreadyExists);
                    return View(vm);
                }

                TempData["Failed"] = DashboardResource.ProjectUpdateFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.ProjectUpdateFailed;
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
                var result = await _projectDomain.DeleteProject(id, userId);
                if (result == 1)
                    TempData["ok"] = DashboardResource.ProjectDeleteSuccess;
                else if (result == -3)
                    TempData["Failed"] = DashboardResource.StatusInUseCannotDelete;
                else
                    TempData["Failed"] = DashboardResource.ProjectDeleteFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.ProjectDeleteFailed;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
