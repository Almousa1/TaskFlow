using TaskFlow.Business.ViewModels;
using TaskFlow.Business.Domain.Common;
using TaskFlow.Data.Models;
using TaskFlow.Data.Repository;

namespace TaskFlow.Business.Domain
{
    public class SystemUserDomain : BaseDomain
    {
        private readonly SystemUserRepository _sysRepo;
        private readonly UserRoleRepository _roleRepo;
        private readonly SystemLogDomain _systemLogDomain;

        public SystemUserDomain(SystemUserRepository systemUserRepository, UserRoleRepository userRoleRepository, SystemLogDomain systemLogDomain)
        {
            _sysRepo = systemUserRepository;
            _roleRepo = userRoleRepository;
            _systemLogDomain = systemLogDomain;
        }

        public async Task<tblSystemUser> GetByGuid(Guid guid)
        {
            return await _sysRepo.GetByGuid(guid);
        }

        public async Task<tblSystemUser> GetByEmail(string email)
        {
            return await _sysRepo.GetByEmailAsync(email);
        }

        public async Task<int> InsertUser(UserCreateVM vm, int UserId)
        {
            var exists = await _sysRepo.GetByEmailAsync(vm.Email);
            if (exists != null)
                return -2;

            var entity = new tblSystemUser
            {
                Email = vm.Email,
                Name = vm.Name,
                NameAr = vm.NameAr,
                Password = _sysRepo.HashPassword(vm.Password),
                UserRoleId = vm.UserRoleId,
            };

            var ok = await _sysRepo.InsertUser(entity);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync(
                   (LogActionType)0,
                tableName: "SystemUser",
                affectedRecord: entity.Id,
                userId: UserId,
                 oldValues: null,
                 newValues: new
                 {
                     entity.Id,
                     entity.guid,
                     entity.Email,
                     entity.Name,
                     entity.NameAr,
                     entity.UserRoleId,
                     entity.IsActive
                 }
              );
            }
            return ok;
        }

        public async Task<int> UpdateUser(UserEditVM vm, int UserId)
        {
            var user = await _sysRepo.GetSystemUserByGuid(vm.Guid);
            if (user == null) return 0;

            var oldvalues = new
            {
                user.Email,
                user.Name,
                user.NameAr,
                user.UserRoleId,
                user.IsActive
            };

            user.Email = vm.Email;
            user.Name = vm.Name;
            user.NameAr = vm.NameAr;
            user.IsActive = vm.IsActive;
            user.UserRoleId = vm.UserRoleId;

            var ok = await _sysRepo.UpdateUser(user);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync(
                   (LogActionType)1,
                tableName: "SystemUser",
                affectedRecord: user.Id,
                userId: UserId,
                 oldValues: oldvalues,
                 newValues: new
                 {
                     user.Id,
                     user.guid,
                     user.Email,
                     user.Name,
                     user.NameAr,
                     user.UserRoleId,
                     user.IsActive
                 }
              );
            }
            return ok;
        }

        public async Task<int> DeleteUser(Guid guid, int UserId)
        {
            var User = await _sysRepo.GetByGuid(guid);
            var ok = await _sysRepo.SoftDeleteByGuidAsync(guid);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync(
                   (LogActionType)2,
                tableName: "SystemUser",
                affectedRecord: User.Id,
                userId: UserId,
                 oldValues: new
                 {
                     User.Id,
                     User.guid,
                     User.Email,
                     User.Name,
                     User.NameAr,
                     User.UserRoleId,
                     User.IsActive
                 },
                 newValues: null
              );
            }
            return ok;
        }

        public async Task<int> GetUsersCount()
        {
            return await _sysRepo.GetUsersCount();
        }

        public async Task<string> GetEmailById(int id)
        {
            return await _sysRepo.GetEmailById(id);
        }

        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return _sysRepo.VerifyPassword(plainPassword, hashedPassword);
        }
    }
}
