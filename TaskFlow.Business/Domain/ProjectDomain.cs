using TaskFlow.Business.Domain.Common;
using TaskFlow.Business.ViewModels;
using TaskFlow.Data.Models;
using TaskFlow.Data.Repository;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Business.Domain
{
    public class ProjectDomain : BaseDomain
    {
        public ProjectRepository _ProjectRepository;
        public SystemLogDomain _systemLogDomain;

        public ProjectDomain(ProjectRepository projectRepository, SystemLogDomain systemLogDomain)
        {
            _ProjectRepository = projectRepository;
            _systemLogDomain = systemLogDomain;
        }

        public async Task<IEnumerable<ProjectListItemVM>> GetProjectListItems(int userId)
        {
            var list = await _ProjectRepository.GetProjects(userId);
            return list.Select(x => new ProjectListItemVM
            {
                Guid = x.guid,
                Name = x.Name,
                Description = x.Description,
                Color = x.Color,
                IsActive = x.IsActive,
                CreationDate = x.CreationDate,
                TaskCount = 0
            });
        }

        public async Task<ProjectEditVM> GetProjectEditViewModel(Guid guid)
        {
            var project = await _ProjectRepository.GetProjectByGuid(guid);
            if (project == null) return null;

            return new ProjectEditVM
            {
                Guid = project.guid,
                Name = project.Name,
                Description = project.Description,
                Color = project.Color,
                IsActive = project.IsActive
            };
        }

        public async Task<int> InsertProject(ProjectCreateVM vm, int userId, int UserId)
        {
            var exists = await _ProjectRepository.ProjectNameExistsForInsert(vm.Name, userId);
            if (exists)
                return -2;

            var project = new tblProject
            {
                Name = vm.Name,
                Description = vm.Description,
                Color = vm.Color,
                UserId = userId
            };

            var result = await _ProjectRepository.InsertProject(project);

            if (result == 1)
                await _systemLogDomain.LogAsync(
                    (LogActionType)0,
                    tableName: "Project",
                    affectedRecord: project.Id,
                    userId: UserId,
                    oldValues: null,
                    newValues: new
                    {
                        vm.Name,
                        vm.Description,
                        vm.Color
                    }
                );

            return result;
        }

        public async Task<int> UpdateProject(ProjectEditVM vm, int UserId)
        {
            var project = await _ProjectRepository.GetProjectByGuid(vm.Guid);
            if (project == null) return 0;

            var exists = await _ProjectRepository.ProjectNameExists(vm.Name, vm.Guid, project.UserId);
            if (exists)
                return -2;

            var oldValues = new
            {
                project.Name,
                project.Description,
                project.Color,
                project.IsActive
            };

            project.Name = vm.Name;
            project.Description = vm.Description;
            project.Color = vm.Color;
            project.IsActive = vm.IsActive;

            var result = await _ProjectRepository.UpdateProject(project);

            if (result == 1)
                await _systemLogDomain.LogAsync(
                    (LogActionType)1,
                    tableName: "Project",
                    affectedRecord: project.Id,
                    userId: UserId,
                    oldValues: oldValues,
                    newValues: new
                    {
                        vm.Name,
                        vm.Description,
                        vm.Color,
                        vm.IsActive
                    }
                );

            return result;
        }

        public async Task<int> DeleteProject(Guid guid, int UserId)
        {
            if (guid == Guid.Empty)
                return 0;

            var project = await _ProjectRepository.GetProjectByGuid(guid);
            if (project == null)
                return 0;

            var ok = await _ProjectRepository.SoftDeleteByGuidAsync(guid);

            if (ok == 1)
                await _systemLogDomain.LogAsync(
                    (LogActionType)2,
                    tableName: "Project",
                    affectedRecord: project.Id,
                    userId: UserId,
                    oldValues: new
                    {
                        project.Name,
                        project.Description,
                        project.Color
                    },
                    newValues: null
                );

            return ok;
        }
    }
}
