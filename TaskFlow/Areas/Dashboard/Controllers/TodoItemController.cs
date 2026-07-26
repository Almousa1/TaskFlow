using TaskFlow.Business.Domain;
using TaskFlow.Business.ViewModels;
using TaskFlow.Data.Repository;
using TaskFlow.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace TaskFlow.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize]
    public class TodoItemController : Controller
    {
        private readonly TodoItemDomain _todoItemDomain;
        private readonly StatusRepository _statusRepository;
        private readonly ProjectRepository _projectRepository;
        private readonly CategoryRepository _categoryRepository;

        public TodoItemController(TodoItemDomain todoItemDomain, StatusRepository statusRepository, ProjectRepository projectRepository, CategoryRepository categoryRepository)
        {
            _todoItemDomain = todoItemDomain;
            _statusRepository = statusRepository;
            _projectRepository = projectRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string status, int? projectId, int? categoryId, string search)
        {
            var userIdStr = User.FindFirstValue("UserId");
            int.TryParse(userIdStr, out var userId);

            int? statusId = null;
            bool? isCompleted = null;
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (status == "completed") isCompleted = true;
                else if (status == "pending") isCompleted = false;
                else if (int.TryParse(status, out var sid)) statusId = sid;
            }

            var model = await _todoItemDomain.GetTodoItemListItems(userId, projectId, categoryId, statusId, isCompleted, search);
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var userIdStr = User.FindFirstValue("UserId");
            int.TryParse(userIdStr, out var userId);

            var vm = new TodoItemCreateVM();
            await PopulateDropdowns(vm, userId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,DueDate,Priority,ProjectId,CategoryId,StatusId")] TodoItemCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                await PopulateDropdowns(vm, userId);
                return View(vm);
            }

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _todoItemDomain.InsertTodoItem(vm, userId, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.TodoInsertSuccess;
                    return RedirectToAction(nameof(Index));
                }

                TempData["Failed"] = DashboardResource.TodoInsertFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.TodoInsertFailed;
            }

            var uidStr = User.FindFirstValue("UserId");
            int.TryParse(uidStr, out var uid);
            await PopulateDropdowns(vm, uid);
            return View(vm);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            var vm = await _todoItemDomain.GetTodoItemEditViewModel(id);
            if (vm == null) return NotFound();

            var userIdStr = User.FindFirstValue("UserId");
            int.TryParse(userIdStr, out var userId);
            await PopulateDropdowns(vm, userId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Guid,Title,Description,DueDate,Priority,ProjectId,CategoryId,StatusId,IsCompleted,IsActive")] TodoItemEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                await PopulateDropdowns(vm, userId);
                return View(vm);
            }

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _todoItemDomain.UpdateTodoItem(vm, userId);
                if (result == 1)
                {
                    TempData["ok"] = DashboardResource.TodoUpdateSuccess;
                    return RedirectToAction(nameof(Index));
                }

                TempData["Failed"] = DashboardResource.TodoUpdateFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.TodoUpdateFailed;
            }

            var uidStr = User.FindFirstValue("UserId");
            int.TryParse(uidStr, out var uid);
            await PopulateDropdowns(vm, uid);
            return View(vm);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                var result = await _todoItemDomain.DeleteTodoItem(id, userId);
                if (result == 1)
                    TempData["ok"] = DashboardResource.TodoDeleteSuccess;
                else
                    TempData["Failed"] = DashboardResource.TodoDeleteFailed;
            }
            catch
            {
                TempData["Failed"] = DashboardResource.TodoDeleteFailed;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ToggleComplete(Guid id)
        {
            if (id == Guid.Empty) return NotFound();

            try
            {
                var userIdStr = User.FindFirstValue("UserId");
                int.TryParse(userIdStr, out var userId);
                await _todoItemDomain.ToggleComplete(id, userId);
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(TodoItemCreateVM vm, int userId)
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var projects = await _projectRepository.GetProjects(userId);
            vm.ProjectOptions = projects.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();

            var categories = await _categoryRepository.GetCategories(userId);
            vm.CategoryOptions = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            var statuses = await _statusRepository.GetStatuses();
            vm.StatusOptions = statuses.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = culture == "ar" ? s.StatusNameAr : s.StatusName
            }).ToList();
        }
    }
}
