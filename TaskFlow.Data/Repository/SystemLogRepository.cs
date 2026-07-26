using TaskFlow.Data;
using TaskFlow.Data.Models;
using TaskFlow.Data.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Data.Repository
{
    public class SystemLogRepository : BaseRepository<tblSystemLog>
    {
        public SystemLogRepository(TaskFlowContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<tblSystemLog>> GetSystemLogs(DateTime? startDate, DateTime? endDate, int? userId = null)
        {
            var query = dbSet.Where(x => !x.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(x => x.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.Timestamp <= endDate.Value.AddDays(1).AddTicks(-1));

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            return await query.ToListAsync();
        }

        public async Task<tblSystemLog> GetSystemLogByGuid(Guid guid)
        {
            return await dbSet.AsNoTracking().SingleOrDefaultAsync(C => C.guid == guid);
        }

        public async Task<int> InsertSystemLog(tblSystemLog systemLog)
        {
            try
            {
                await InsertAsync(systemLog);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
