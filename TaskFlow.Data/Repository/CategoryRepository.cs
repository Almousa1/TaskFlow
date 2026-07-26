using TaskFlow.Data.Repository.Common;
using TaskFlow.Data.Models;
using TaskFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Data.Repository
{
    public class CategoryRepository : BaseRepository<tblCategory>
    {
        public TodoItemRepository _todoItemRepository;
        public CategoryRepository(TaskFlowContext context, TodoItemRepository todoItemRepository) : base(context)
        {
            _todoItemRepository = todoItemRepository;
        }

        public async Task<IEnumerable<tblCategory>> GetCategories(int userId)
        {
            return await dbSet
                .Where(c => !c.IsDeleted && c.IsActive && c.UserId == userId)
                .Include(c => c.User)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<tblCategory> GetCategoryByGuid(Guid guid)
        {
            return await dbSet
                .Include(c => c.User)
                .Where(c => c.guid == guid && !c.IsDeleted && c.IsActive)
                .SingleOrDefaultAsync();
        }

        public async Task<tblCategory> GetCategoryById(int id)
        {
            if (id <= 0)
                return null;

            return await dbSet
                .AsNoTracking()
                .Where(c => c.Id == id && !c.IsDeleted && c.IsActive)
                .SingleOrDefaultAsync();
        }

        public async Task<int> InsertCategory(tblCategory category)
        {
            if (category == null)
                return 0;
            try
            {
                var result = await InsertAsync(category);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateCategory(tblCategory category)
        {
            if (await UpdateAsync(category))
                return 1;
            return 0;
        }

        public async Task<int> SoftDeleteByGuidAsync(Guid guid)
        {
            var entity = await dbSet.SingleOrDefaultAsync(c => c.guid == guid);
            if (entity == null)
                return 0;

            var inUse = await _todoItemRepository.dbSet.AnyAsync(t => t.CategoryId == entity.Id && !t.IsDeleted);
            if (inUse)
                return -3;

            Delete(entity);
            return 1;
        }

        public async Task<bool> CategoryNameExists(string name, Guid guid, int userId)
        {
            return await dbSet.Where(c => c.Name == name && c.UserId == userId && !c.IsDeleted && c.guid != guid).AnyAsync();
        }
        public async Task<bool> CategoryNameExistsForInsert(string name, int userId)
        {
            return await dbSet.Where(c => c.Name == name && c.UserId == userId && !c.IsDeleted).AnyAsync();
        }
    }
}
