using TaskFlow.Business.Domain.Common;
using TaskFlow.Business.ViewModels;
using TaskFlow.Data.Models;
using TaskFlow.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskFlow.Business.Domain
{
    public class CategoryDomain : BaseDomain
    {
        public CategoryRepository _CategoryRepository;
        public SystemLogDomain _systemLogDomain;

        public CategoryDomain(CategoryRepository categoryRepository, SystemLogDomain systemLogDomain)
        {
            _CategoryRepository = categoryRepository;
            _systemLogDomain = systemLogDomain;
        }

        public async Task<IEnumerable<CategoryListItemVM>> GetCategoryListItems(int userId)
        {
            var list = await _CategoryRepository.GetCategories(userId);
            return list.Select(x => new CategoryListItemVM
            {
                Guid = x.guid,
                Name = x.Name,
                Color = x.Color,
                IsActive = x.IsActive,
                CreationDate = x.CreationDate,
                TaskCount = 0
            });
        }

        public async Task<CategoryEditVM> GetCategoryEditViewModel(Guid guid)
        {
            var category = await _CategoryRepository.GetCategoryByGuid(guid);
            if (category == null) return null;

            return new CategoryEditVM
            {
                Guid = category.guid,
                Name = category.Name,
                Color = category.Color,
                IsActive = category.IsActive
            };
        }

        public async Task<int> InsertCategory(CategoryCreateVM vm, int userId, int UserId)
        {
            var exists = await _CategoryRepository.CategoryNameExistsForInsert(vm.Name, userId);
            if (exists)
                return -2;

            var entity = new tblCategory
            {
                Name = vm.Name,
                Color = vm.Color,
                UserId = userId
            };

            var result = await _CategoryRepository.InsertCategory(entity);

            if (result == 1)
                await _systemLogDomain.LogAsync(
                    (LogActionType)0,
                    tableName: "Category",
                    affectedRecord: entity.Id,
                    userId: UserId,
                    oldValues: null,
                    newValues: new
                    {
                        vm.Name,
                        vm.Color
                    }
                );

            return result;
        }

        public async Task<int> UpdateCategory(CategoryEditVM vm, int UserId)
        {
            var category = await _CategoryRepository.GetCategoryByGuid(vm.Guid);
            if (category == null) return 0;

            var exists = await _CategoryRepository.CategoryNameExists(vm.Name, vm.Guid, category.UserId);
            if (exists)
                return -2;

            var oldValues = new
            {
                category.Name,
                category.Color,
                category.IsActive
            };

            category.Name = vm.Name;
            category.Color = vm.Color;
            category.IsActive = vm.IsActive;

            var result = await _CategoryRepository.UpdateCategory(category);

            if (result == 1)
                await _systemLogDomain.LogAsync(
                    (LogActionType)1,
                    tableName: "Category",
                    affectedRecord: category.Id,
                    userId: UserId,
                    oldValues: oldValues,
                    newValues: new
                    {
                        vm.Name,
                        vm.Color,
                        vm.IsActive
                    }
                );

            return result;
        }

        public async Task<int> DeleteCategory(Guid guid, int UserId)
        {
            if (guid == Guid.Empty)
                return 0;

            var category = await _CategoryRepository.GetCategoryByGuid(guid);
            if (category == null)
                return 0;

            var ok = await _CategoryRepository.SoftDeleteByGuidAsync(guid);

            if (ok == 1)
                await _systemLogDomain.LogAsync(
                    (LogActionType)2,
                    tableName: "Category",
                    affectedRecord: category.Id,
                    userId: UserId,
                    oldValues: new
                    {
                        category.Name,
                        category.Color
                    },
                    newValues: null
                );

            return ok;
        }
    }
}
