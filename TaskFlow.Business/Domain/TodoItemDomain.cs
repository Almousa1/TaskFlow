using TaskFlow.Business.Domain.Common;
using TaskFlow.Business.ViewModels;
using TaskFlow.Data.Models;
using TaskFlow.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskFlow.Business.Domain
{
    public class TodoItemDomain : BaseDomain
    {
        private readonly TodoItemRepository _todoItemRepository;
        private readonly StatusRepository _statusRepository;
        private readonly SystemLogDomain _systemLogDomain;

        public TodoItemDomain(TodoItemRepository todoItemRepository, StatusRepository statusRepository, SystemLogDomain systemLogDomain)
        {
            _todoItemRepository = todoItemRepository;
            _statusRepository = statusRepository;
            _systemLogDomain = systemLogDomain;
        }

        public async Task<IEnumerable<TodoItemListItemVM>> GetTodoItemListItems(int userId, int? projectId = null, int? categoryId = null, int? statusId = null, bool? isCompleted = null, string search = null)
        {
            var list = await _todoItemRepository.GetTodoItems(userId);

            if (projectId.HasValue)
                list = list.Where(t => t.ProjectId == projectId.Value).ToList();

            if (categoryId.HasValue)
                list = list.Where(t => t.CategoryId == categoryId.Value).ToList();

            if (statusId.HasValue)
                list = list.Where(t => t.StatusId == statusId.Value).ToList();

            if (isCompleted.HasValue)
                list = list.Where(t => t.IsCompleted == isCompleted.Value).ToList();

            if (!string.IsNullOrWhiteSpace(search))
                list = list.Where(t => t.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (t.Description != null && t.Description.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();

            return list.Select(t => new TodoItemListItemVM
            {
                Guid = t.guid,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                Priority = t.Priority,
                ProjectName = t.Project?.Name,
                CategoryName = t.Category?.Name,
                StatusName = t.Status?.StatusName,
                StatusNameAr = t.Status?.StatusNameAr,
                CreationDate = t.CreationDate
            });
        }

        public async Task<TodoItemEditVM> GetTodoItemEditViewModel(Guid guid)
        {
            var item = await _todoItemRepository.GetTodoItemByGuid(guid);
            if (item == null) return null;

            var culture = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var statuses = await _statusRepository.GetStatuses();

            var vm = new TodoItemEditVM
            {
                Guid = item.guid,
                Title = item.Title,
                Description = item.Description,
                DueDate = item.DueDate,
                Priority = item.Priority,
                ProjectId = item.ProjectId,
                CategoryId = item.CategoryId,
                StatusId = item.StatusId,
                IsCompleted = item.IsCompleted,
                IsActive = item.IsActive,
                StatusOptions = statuses.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = culture == "ar" ? s.StatusNameAr : s.StatusName
                }).ToList()
            };

            return vm;
        }

        public async Task<int> InsertTodoItem(TodoItemCreateVM vm, int userId, int UserId)
        {
            if (vm == null)
                return 0;

            var entity = new tblTodoItem
            {
                Title = vm.Title,
                Description = vm.Description,
                DueDate = vm.DueDate,
                Priority = vm.Priority,
                ProjectId = vm.ProjectId,
                CategoryId = vm.CategoryId,
                StatusId = vm.StatusId,
                UserId = userId
            };

            var ok = await _todoItemRepository.InsertTodoItem(entity);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync((LogActionType)0,
                tableName: "TodoItem",
                affectedRecord: entity.Id,
                userId: UserId,
                oldValues: null,
                newValues: new
                {
                    entity.Title,
                    entity.Description,
                    entity.DueDate,
                    entity.Priority,
                    entity.ProjectId,
                    entity.CategoryId,
                    entity.StatusId
                });
            }
            return ok;
        }

        public async Task<int> UpdateTodoItem(TodoItemEditVM vm, int UserId)
        {
            if (vm.Guid == Guid.Empty)
                return 0;

            var existing = await _todoItemRepository.GetTodoItemByGuid(vm.Guid);
            if (existing == null)
                return 0;

            var old = new
            {
                existing.Title,
                existing.Description,
                existing.DueDate,
                existing.Priority,
                existing.ProjectId,
                existing.CategoryId,
                existing.StatusId,
                existing.IsCompleted,
                existing.IsActive
            };

            existing.Title = vm.Title;
            existing.Description = vm.Description;
            existing.DueDate = vm.DueDate;
            existing.Priority = vm.Priority;
            existing.ProjectId = vm.ProjectId;
            existing.CategoryId = vm.CategoryId;
            existing.StatusId = vm.StatusId;
            existing.IsCompleted = vm.IsCompleted;
            existing.IsActive = vm.IsActive;

            var ok = await _todoItemRepository.UpdateTodoItem(existing);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync((LogActionType)1,
                tableName: "TodoItem",
                affectedRecord: existing.Id,
                userId: UserId,
                oldValues: old,
                newValues: new
                {
                    vm.Title,
                    vm.Description,
                    vm.DueDate,
                    vm.Priority,
                    vm.ProjectId,
                    vm.CategoryId,
                    vm.StatusId,
                    vm.IsCompleted,
                    vm.IsActive
                });
            }
            return ok;
        }

        public async Task<int> DeleteTodoItem(Guid guid, int UserId)
        {
            if (guid == Guid.Empty)
                return 0;

            var item = await _todoItemRepository.GetTodoItemByGuid(guid);
            if (item == null)
                return 0;

            var ok = await _todoItemRepository.SoftDeleteByGuidAsync(guid);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync((LogActionType)2,
                tableName: "TodoItem",
                affectedRecord: item.Id,
                userId: UserId,
                oldValues: new
                {
                    item.Title,
                    item.Description,
                    item.DueDate,
                    item.Priority,
                    item.ProjectId,
                    item.CategoryId,
                    item.StatusId
                },
                newValues: null);
            }
            return ok;
        }

        public async Task<int> ToggleComplete(Guid guid, int UserId)
        {
            if (guid == Guid.Empty)
                return 0;

            var item = await _todoItemRepository.GetTodoItemByGuid(guid);
            if (item == null)
                return 0;

            var old = new { item.IsCompleted };

            item.IsCompleted = !item.IsCompleted;

            var ok = await _todoItemRepository.UpdateTodoItem(item);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync((LogActionType)1,
                tableName: "TodoItem",
                affectedRecord: item.Id,
                userId: UserId,
                oldValues: old,
                newValues: new { item.IsCompleted });
            }
            return ok;
        }

        public async Task<DashboardVM> GetStats(int userId)
        {
            var total = await _todoItemRepository.GetTodoItemsCount(userId);
            var completed = await _todoItemRepository.GetCompletedCount(userId);
            var pending = await _todoItemRepository.GetPendingCount(userId);
            var items = await _todoItemRepository.GetTodoItems(userId);
            var overdue = items.Count(t => !t.IsCompleted && t.DueDate.HasValue && t.DueDate.Value < DateTime.Now);

            return new DashboardVM
            {
                TotalTasks = total,
                CompletedTasks = completed,
                PendingTasks = pending,
                OverdueTasks = overdue
            };
        }
    }
}
