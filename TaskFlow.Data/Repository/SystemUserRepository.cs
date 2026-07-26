using TaskFlow.Data.Repository.Common;
using TaskFlow.Data.Models;
using TaskFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Data.Repository
{
    public class SystemUserRepository : BaseRepository<tblSystemUser>
    {

        public SystemUserRepository(TaskFlowContext dbContext) : base(dbContext)
        {

        }

        public async Task<tblSystemUser> GetByGuid(Guid guid)
        {
            return await dbSet
                .Where(s => !s.IsDeleted && s.guid == guid)
                .Include(s => s.UserRole)
                .SingleOrDefaultAsync();
        }



        public async Task<tblSystemUser> GetByEmailAsync(string email)
        {
            return await dbSet.AsNoTracking()
                .Where(s => !s.IsDeleted && s.Email.Trim() == email.Trim())
                .Include(s => s.UserRole)
                .FirstOrDefaultAsync();
        }

        public async Task<tblSystemUser> GetSystemUserByGuid(Guid guid)
        {
            return await dbSet.Where(c => c.guid == guid)
                .SingleOrDefaultAsync();
        }

        public async Task<int> InsertUser(tblSystemUser systemUser)
        {
            try
            {
                await InsertAsync(systemUser);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> UpdateUser(tblSystemUser user)
        {
            try
            {
                await UpdateAsync(user);
                return 1;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> SoftDeleteByGuidAsync(Guid guid)
        {

            var entity = await dbSet.SingleOrDefaultAsync(ss => ss.guid == guid);
            if (entity == null)
            {
                return 0;
            }
            else
            {
                Delete(entity);
                return 1;
            }

        }

        public async Task<tblSystemUser> FindKfuByEmailAsync(string email)
        {
            return await dbSet
                .AsNoTracking()
                .Include(u => u.UserRole)
                .SingleOrDefaultAsync(u => !u.IsDeleted && u.Email == email && u.IsActive);
        }

        public async Task<int?> FindKfuIdByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var user = await dbSet
                .AsNoTracking()
                .SingleOrDefaultAsync(u => !u.IsDeleted && u.Email == email && u.IsActive);

            return user?.Id;
        }

        public async Task<IEnumerable<tblSystemUser>> GetUsersWithRoleAsync()
        {
            return await dbSet
                .Where(p => !p.IsDeleted)
                .Include(p => p.UserRole)
                .ToListAsync();
        }
        public async Task<int> GetUsersCount()
        {
            return await dbSet
                .Where(p => !p.IsDeleted)
                .CountAsync();
        }
        public async Task<int> GetAdminsCount()
        {
            return await dbSet
                .Where(p => !p.IsDeleted && p.UserRoleId == 1)
                .CountAsync();
        }
        public async Task<IEnumerable<tblSystemUser>> GetUsersIdWithRole()
        {
            return await dbSet
                .Where(p => !p.IsDeleted && p.UserRoleId == 1)
                .Include(p => p.UserRole)
                .ToListAsync();
        }
        public async Task<tblSystemUser> GetByIdAsync(int id)
        {
            return await dbSet
                .Where(s => !s.IsDeleted && s.IsActive && s.Id == id)
                .Include(s => s.UserRole)
                .SingleOrDefaultAsync();

        }
        public async Task<List<tblSystemUser>> GetAllInActiveAsync()
        {
            return await dbSet
                .Where(s => !s.IsDeleted && !s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }
        public async Task<string> GetEmailById(int id)
        {
            return await dbSet.Where(s => s.Id == id).Select(s => s.Email).SingleOrDefaultAsync();
        }
        public async Task<bool> GetUserStatusByGuidAsync(Guid guid)
        {
            return await dbSet.Where(s => s.guid == guid && !s.IsDeleted && s.IsActive).AnyAsync();

        }
    }
}
