using TaskFlow.Business.ViewModels;
using TaskFlow.Business.Domain.Common;
using TaskFlow.Data.Models;
using TaskFlow.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskFlow.Business.Domain
{
    public class UserRoleDomain : BaseDomain
    {
        public UserRoleRepository _UserRoleRepository;
        public readonly SystemLogDomain _systemLogDomain;

        public UserRoleDomain(SystemLogDomain systemLogDomain, UserRoleRepository userRoleRepository)
        {
            _systemLogDomain = systemLogDomain;
            _UserRoleRepository = userRoleRepository;
        }

        public async Task<IEnumerable<UserRoleListItemVM>> GetListAsync()
        {
            var list = await _UserRoleRepository.GetAllActiveAsync();
            return list.Select(x => new UserRoleListItemVM
            {
                Guid = x.guid,
                RoleName = x.RoleName,
                RoleNameAr = x.RoleNameAr,
                CreationDate = x.CreationDate,
                IsActive = x.IsActive
            });
        }

        public async Task<int> InsertAsync(UserRoleCreateVM vm, int UserId)
        {
            var exists = await _UserRoleRepository.IsRoleNameExists(vm.RoleName, vm.RoleNameAr);
            if (exists)
                return 0;

            var exists2 = await _UserRoleRepository.IsRoleNameExistsAr(vm.RoleName, vm.RoleNameAr);
            if (exists2)
                return 2;

            var entity = new tblUserRole
            {
                RoleName = vm.RoleName,
                RoleNameAr = vm.RoleNameAr,
            };

            var ok = await _UserRoleRepository.InsertUserRole(entity);
            if (ok)
            {
                await _systemLogDomain.LogAsync(
                  (LogActionType)0,
               tableName: "UserRole",
               affectedRecord: entity.Id,
               userId: UserId,
                oldValues: null,
                newValues: new
                {
                   entity.guid,
                   entity.RoleName,
                   entity.RoleNameAr
                }
             );
            }
            return ok ? 1 : 0;
        }

        public async Task<UserRoleEditVM> GetForEditAsync(Guid guid)
        {
            var e = await _UserRoleRepository.GetByGuidAsync(guid);
            if (e == null) return null;

            return new UserRoleEditVM
            {
                Id = e.Id,
                Guid = e.guid,
                RoleName = e.RoleName,
                RoleNameAr = e.RoleNameAr,
                IsActive = e.IsActive,
            };
        }

        public async Task<int> UpdateAsync(UserRoleEditVM vm, int UserId)
        {
            var exists = await _UserRoleRepository.IsRoleNameExistsForUpdat(vm.RoleName, vm.RoleNameAr, vm.Guid);
            if (exists)
                return 0;

            var exists2 = await _UserRoleRepository.IsRoleNameExistsArForUpdat(vm.RoleName, vm.RoleNameAr, vm.Guid);
            if (exists2)
                return 2;

            var e = await _UserRoleRepository.GetByGuidAsync(vm.Guid);
            if (e == null) return 0;

            var oldvalues = new
            {
                e.RoleName,
                e.RoleNameAr,
                e.IsActive
            };

            e.RoleName = vm.RoleName;
            e.RoleNameAr = vm.RoleNameAr;
            e.IsActive = vm.IsActive;

            var ok = await _UserRoleRepository.UpdateUserRole(e);
            if (ok)
            {
                await _systemLogDomain.LogAsync(
                  (LogActionType)1,
               tableName: "UserRole",
               affectedRecord: e.Id,
               userId: UserId,
                oldValues: oldvalues,
                newValues: new
                {
                    e.guid,
                    e.RoleName,
                    e.RoleNameAr,
                    e.IsActive,
                }
             );
            }
            return ok ? 1 : 0;
        }

        public async Task<List<SelectListItem>> GetRoleOptions()
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var userRole = await _UserRoleRepository.GetAllActiveAsync();

            return userRole.Select(c => new SelectListItem
            {
                Value = c.guid.ToString(),
                Text = culture == "ar" ? c.RoleNameAr : c.RoleName,
            }).ToList();
        }

        public async Task<int> DeleteAsync(Guid guid, int UserId)
        {
            var e = await _UserRoleRepository.GetByGuidAsync(guid);
            var ok = await _UserRoleRepository.SoftDeleteByGuidAsync(guid);
            if (ok == 1 && e != null)
            {
                await _systemLogDomain.LogAsync(
                  (LogActionType)2,
               tableName: "UserRole",
               affectedRecord: e.Id,
               userId: UserId,
                oldValues: new
                {
                    e.guid,
                    e.RoleName,
                    e.RoleNameAr
                },
                newValues: null
             );
            }
            if (ok == -2)
                return -2;

            return ok == 1 ? 1 : 0;
        }

        public async Task<int> GetIdByGuidAsync(Guid guid)
        {
            return await _UserRoleRepository.GetIdByGuidAsync(guid);
        }
    }
}
