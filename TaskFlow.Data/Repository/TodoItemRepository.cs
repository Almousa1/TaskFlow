using TaskFlow.Data.Repository.Common;
using TaskFlow.Data.Models;
using TaskFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Data.Repository
{
    public class TodoItemRepository : BaseRepository<tblTodoItem>
    {
        public TodoItemRepository(TaskFlowContext context) : base(context)
        {
        }

        public async Task<List<tblTodoItem>> GetTodoItems(int userId)
        {
            return await dbSet
                .Include(t => t.Project)
                .Include(t => t.Category)
                .Include(t => t.Status)
                .Include(t => t.User)
                .Where(t => !t.IsDeleted && t.UserId == userId)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }

        public async Task<tblTodoItem> GetTodoItemByGuid(Guid guid)
        {
            return await dbSet
                .Include(t => t.Project)
                .Include(t => t.Category)
                .Include(t => t.Status)
                .Include(t => t.User)
                .Where(t => t.guid == guid && !t.IsDeleted)
                .SingleOrDefaultAsync();
        }

        public async Task<tblTodoItem> GetTodoItemById(int id)
        {
            if (id <= 0)
                return null;

            return await dbSet
                .Include(t => t.Project)
                .Include(t => t.Category)
                .Include(t => t.Status)
                .Include(t => t.User)
                .AsNoTracking()
                .Where(t => t.Id == id && !t.IsDeleted)
                .SingleOrDefaultAsync();
        }

        public async Task<int> InsertTodoItem(tblTodoItem todoItem)
        {
            try
            {
                await InsertAsync(todoItem);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateTodoItem(tblTodoItem todoItem)
        {
            try
            {
                await UpdateAsync(todoItem);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> SoftDeleteByGuidAsync(Guid guid)
        {
            var entity = await dbSet.SingleOrDefaultAsync(t => t.guid == guid);
            if (entity == null)
                return 0;

            Delete(entity);
            return 1;
        }

        public async Task<List<tblTodoItem>> GetTodoItemsByProjectId(int projectId)
        {
            return await dbSet
                .Include(t => t.Status)
                .Where(t => t.ProjectId == projectId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<tblTodoItem>> GetTodoItemsByCategoryId(int categoryId)
        {
            return await dbSet
                .Include(t => t.Status)
                .Where(t => t.CategoryId == categoryId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<tblTodoItem>> GetTodoItemsByStatusId(int statusId)
        {
            return await dbSet
                .Include(t => t.Project)
                .Include(t => t.Category)
                .Where(t => t.StatusId == statusId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<int> GetTodoItemsCount(int userId)
        {
            return await dbSet
                .Where(t => !t.IsDeleted && t.UserId == userId)
                .CountAsync();
        }

        public async Task<int> GetCompletedCount(int userId)
        {
            return await dbSet
                .Where(t => !t.IsDeleted && t.UserId == userId && t.IsCompleted)
                .CountAsync();
        }

        public async Task<int> GetPendingCount(int userId)
        {
            return await dbSet
                .Where(t => !t.IsDeleted && t.UserId == userId && !t.IsCompleted)
                .CountAsync();
        }
    }
}
