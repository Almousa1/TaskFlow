using TaskFlow.Business.ViewModels;
using TaskFlow.Business.Domain.Common;
using TaskFlow.Data.Models;
using TaskFlow.Data.Repository;

namespace TaskFlow.Business.Domain
{
    public class StatusDomain : BaseDomain
    {
        private readonly StatusRepository _statusRepository;
        private readonly SystemLogDomain _systemLogDomain;

        public StatusDomain(StatusRepository statusRepository, SystemLogDomain systemLogDomain)
        {
            _statusRepository = statusRepository;
            _systemLogDomain = systemLogDomain;
        }

        public async Task<IEnumerable<StatusListItemVM>> GetStatusListItems()
        {
            var list = await _statusRepository.GetStatuses();
            return list.Select(status => new StatusListItemVM
            {
                Guid = status.guid,
                StatusName = status.StatusName,
                StatusNameAr = status.StatusNameAr,
                Approved = status.IsActive,
                UpdatedAt = status.CreationDate
            });
        }

        public async Task<StatusListItemVM> GetStatusByGuidViewModel(Guid guid)
        {
            if (guid == Guid.Empty)
                return null;

            var status = await _statusRepository.GetStatusByGuid(guid);
            if (status == null)
                return null;

            return new StatusListItemVM
            {
                Guid = status.guid,
                StatusName = status.StatusName,
                StatusNameAr = status.StatusNameAr,
                Approved = status.IsActive,
                UpdatedAt = status.CreationDate
            };
        }

        public async Task<StatusEditVM> GetStatusEditViewModel(Guid guid)
        {
            if (guid == Guid.Empty)
                return null;

            var status = await _statusRepository.GetStatusByGuid(guid);
            if (status == null)
                return null;

            return new StatusEditVM
            {
                Guid = status.guid,
                StatusName = status.StatusName,
                StatusNameAr = status.StatusNameAr,
                Approved = status.IsActive
            };
        }

        public async Task<int> InsertStatus(StatusCreateVM statusVm, int UserId)
        {
            if (statusVm == null)
                return 0;

            var exists = await _statusRepository.StatusNameExistsForInsert(statusVm.StatusName, statusVm.StatusNameAr);
            if (exists)
                return -2;

            tblStatus entity = new tblStatus
            {
                StatusName = statusVm.StatusName,
                StatusNameAr = statusVm.StatusNameAr,
            };

            var ok = await _statusRepository.InsertStatus(entity);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync((LogActionType)0,
                tableName: "Status",
                affectedRecord: entity.Id,
                userId: UserId,
                oldValues: null,
                newValues: new
                {
                    statusVm.StatusName,
                    statusVm.StatusNameAr
                });
            }
            return ok;
        }

        public async Task<int> UpdateStatus(StatusEditVM statusVm, int UserId)
        {
            if (statusVm.Guid == Guid.Empty)
                return 0;
            var existing = await _statusRepository.GetStatusByGuid(statusVm.Guid);

            var exists = await _statusRepository.StatusNameExists(statusVm.StatusName, statusVm.StatusNameAr, statusVm.Guid);
            if (exists)
                return -2;

            var old = new
            {
                existing.StatusName,
                existing.StatusNameAr,
                existing.IsActive
            };

            existing.StatusName = statusVm.StatusName;
            existing.StatusNameAr = statusVm.StatusNameAr;
            existing.IsActive = statusVm.Approved;

            var ok = await _statusRepository.UpdateStatus(existing);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync((LogActionType)1,
                tableName: "Status",
                affectedRecord: existing.Id,
                userId: UserId,
                oldValues: old,
                newValues: new
                {
                    statusVm.StatusName,
                    statusVm.StatusNameAr,
                    statusVm.Approved
                });
            }
            return ok;
        }

        public async Task<int> DeleteStatus(Guid guid, int UserId)
        {
            if (guid == Guid.Empty)
                return 0;

            var status = await _statusRepository.GetStatusByGuid(guid);
            if (status == null)
                return 0;

            var ok = await _statusRepository.DeleteStatus(guid);
            if (ok == 1)
            {
                await _systemLogDomain.LogAsync((LogActionType)2,
                tableName: "Status",
                affectedRecord: status.Id,
                userId: UserId,
                oldValues: new
                {
                    status.StatusName,
                    status.StatusNameAr
                },
                newValues: null);
            }
            return ok;
        }
    }
}
