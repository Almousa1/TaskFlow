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
    public class CategoryController : Controller
    {
        private readonly CategoryDomain _categoryDomain;

        public CategoryController(CategoryDomain categoryDomain)
        {
            _categoryDomain = categoryDomain;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue("UserId");
            int.TryParse(userIdStr, out var userId);
            var model = await _categoryDomain.GetCategoryListItems(userId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Color")] CategoryCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _categoryDomain.InsertCategory(vm, userId, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.CategoryInsertSuccess;
                    return RedirectToAction(nameof(Index));
                }
                if (result == -2)
                {
                    ModelState.AddModelError("Name", DashboardResource.StatusAlreadyExists);
                    return View(vm);
                }

                TempData["Failed"] = DashboardResource.CategoryInsertFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.CategoryInsertFailed;
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var vm = await _categoryDomain.GetCategoryEditViewModel(id);
            if (vm == null)
                return NotFound();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Guid,Name,Color,IsActive")] CategoryEditVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _categoryDomain.UpdateCategory(vm, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.CategoryUpdateSuccess;
                    return RedirectToAction(nameof(Index));
                }
                if (result == -2)
                {
                    ModelState.AddModelError("Name", DashboardResource.StatusAlreadyExists);
                    return View(vm);
                }

                TempData["Failed"] = DashboardResource.CategoryUpdateFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.CategoryUpdateFailed;
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
                var result = await _categoryDomain.DeleteCategory(id, userId);
                if (result == 1)
                    TempData["ok"] = DashboardResource.CategoryDeleteSuccess;
                else if (result == -3)
                    TempData["Failed"] = DashboardResource.StatusInUseCannotDelete;
                else
                    TempData["Failed"] = DashboardResource.CategoryDeleteFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.CategoryDeleteFailed;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
