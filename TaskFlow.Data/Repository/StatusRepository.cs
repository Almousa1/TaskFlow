using TaskFlow.Data.Repository.Common;
using TaskFlow.Data.Models;
using TaskFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Data.Repository
{
    public class StatusRepository : BaseRepository<tblStatus>
    {
        public TodoItemRepository _todoItemRepository;
        public StatusRepository(TaskFlowContext context, TodoItemRepository todoItemRepository) : base(context)
        {
            _todoItemRepository = todoItemRepository;
        }

        public async Task<List<tblStatus>> GetStatuses()
        {
            return await dbSet
                .Where(s => s.IsDeleted == false)
                .OrderBy(s => s.StatusName)
                .ToListAsync();
        }

        public async Task<tblStatus> GetStatusByGuid(Guid guid)
        {
            return await dbSet
                .AsNoTracking()
                .Where(s => s.guid == guid && !s.IsDeleted)
                .SingleOrDefaultAsync();
        }

        public async Task<tblStatus> GetStatusById(int id)
        {
            if (id <= 0)
                return null;

            return await dbSet
                .AsNoTracking()
                .Where(s => s.Id == id && !s.IsDeleted)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> StatusNameExists(string statusName, string statusNameAr, Guid guid)
        {
            return await dbSet.Where(s => (s.StatusName == statusName || s.StatusNameAr == statusNameAr) && !s.IsDeleted && s.guid != guid).AnyAsync();
        }
        public async Task<bool> StatusNameExistsForInsert(string statusName, string statusNameAr)
        {
            return await dbSet.Where(s => (s.StatusName == statusName || s.StatusNameAr == statusNameAr) &&
                !s.IsDeleted).AnyAsync();
        }

        public async Task<int> InsertStatus(tblStatus status)
        {
            try
            {
                await InsertAsync(status);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateStatus(tblStatus status)
        {
            try
            {
                await UpdateAsync(status);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> DeleteStatus(Guid guid)
        {
            var status = await dbSet.SingleOrDefaultAsync(s => s.guid == guid);
            var inUse = await _todoItemRepository.dbSet.AnyAsync(t => t.StatusId == status.Id && !t.IsDeleted);

            if (inUse)
                return -3;

            Delete(status);
            return 1;
        }
    }
}
