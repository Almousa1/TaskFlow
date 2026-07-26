using TaskFlow.Data.Models;
using TaskFlow.Data.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Data.Repository
{
    public class UserRoleRepository : BaseRepository<tblUserRole>
    {
        public SystemUserRepository _userRepository;
        public UserRoleRepository(TaskFlowContext dbContext, SystemUserRepository userRepository) : base(dbContext)
        {
            _userRepository = userRepository;
        }

        public async Task<List<tblUserRole>> GetAllActiveAsync()
        {
            return await dbSet
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.RoleName)
                .ToListAsync();
        }

        public async Task<tblUserRole> GetByGuidAsync(Guid guid)
        {
            return await dbSet
                .Where(r => !r.IsDeleted && r.guid == guid)
                .FirstOrDefaultAsync();
        }
        public async Task<int> GetIdByGuidAsync(Guid guid)
        {
            return await dbSet
                .Where(r => !r.IsDeleted && r.IsActive && r.guid == guid).Select(r => r.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> InsertUserRole(tblUserRole e)
        {
            return await InsertAsync(e);
        }

        public async Task<bool> UpdateUserRole(tblUserRole e)
        {
            return await UpdateAsync(e);
        }

        public async Task<int> SoftDeleteByGuidAsync(Guid guid)
        {
            var entity = await dbSet.Where(ss => ss.guid == guid).FirstOrDefaultAsync();
            var inUse = await _userRepository.dbSet.AnyAsync(st => st.UserRoleId == entity.Id && !st.IsDeleted);
            if (entity == null)
            {
                return 0;
            }
            if (inUse)
                return -2;

            Delete(entity);
            return 1;
        }

        public async Task<tblUserRole> FindByNameAsync(string roleName)
        {
            return await dbSet.Where(r => r.RoleName == roleName && !r.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<tblUserRole> GetRoleNameByIdAsync(int roleId)
        {
            return await dbSet.Where(r => r.Id == roleId && !r.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsRoleNameExistsAr(string roleName, string rolenamear)
        {
            return await dbSet.AnyAsync(r => r.RoleNameAr == rolenamear && r.IsActive && !r.IsDeleted);
        }
        public async Task<bool> IsRoleNameExists(string roleName, string rolenamear)
        {
            return await dbSet.AnyAsync(r => r.RoleName == roleName && r.IsActive && !r.IsDeleted);
        }
        public async Task<bool> IsRoleNameExistsArForUpdat(string roleName, string rolenamear, Guid guid)
        {
            return await dbSet.AnyAsync(r => r.RoleNameAr == rolenamear && r.guid != guid && r.IsActive && !r.IsDeleted);
        }
        public async Task<bool> IsRoleNameExistsForUpdat(string roleName, string rolenamear, Guid guid)
        {
            return await dbSet.AnyAsync(r => r.RoleName == roleName && r.guid != guid && r.IsActive && !r.IsDeleted);
        }
    }
}
