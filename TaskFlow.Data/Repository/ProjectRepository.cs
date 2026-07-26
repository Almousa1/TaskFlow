using TaskFlow.Data.Repository.Common;
using TaskFlow.Data.Models;
using TaskFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Data.Repository
{
    public class ProjectRepository : BaseRepository<tblProject>
    {
        public TodoItemRepository _todoItemRepository;
        public ProjectRepository(TaskFlowContext context, TodoItemRepository todoItemRepository) : base(context)
        {
            _todoItemRepository = todoItemRepository;
        }

        public async Task<IEnumerable<tblProject>> GetProjects(int userId)
        {
            return await dbSet
                .Where(p => !p.IsDeleted && p.IsActive && p.UserId == userId)
                .Include(p => p.User)
                .AsNoTracking()
                .OrderByDescending(p => p.CreationDate)
                .ToListAsync();
        }

        public async Task<tblProject> GetProjectByGuid(Guid guid)
        {
            return await dbSet
                .Include(p => p.User)
                .Where(p => p.guid == guid && !p.IsDeleted && p.IsActive)
                .SingleOrDefaultAsync();
        }

        public async Task<tblProject> GetProjectById(int projectId)
        {
            var project = await dbSet
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == projectId && !p.IsDeleted && p.IsActive);
            return project;
        }

        public async Task<int> InsertProject(tblProject project)
        {
            if (project == null)
                return 0;
            try
            {
                var result = await InsertAsync(project);
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateProject(tblProject project)
        {
            if (await UpdateAsync(project))
                return 1;
            return 0;
        }

        public async Task<int> SoftDeleteByGuidAsync(Guid guid)
        {
            var entity = await dbSet.SingleOrDefaultAsync(p => p.guid == guid);
            if (entity == null)
                return 0;

            var inUse = await _todoItemRepository.dbSet.AnyAsync(t => t.ProjectId == entity.Id && !t.IsDeleted);
            if (inUse)
                return -3;

            Delete(entity);
            return 1;
        }

        public async Task<bool> ProjectNameExists(string name, Guid guid, int userId)
        {
            return await dbSet.Where(p => p.Name == name && p.UserId == userId && !p.IsDeleted && p.guid != guid).AnyAsync();
        }
        public async Task<bool> ProjectNameExistsForInsert(string name, int userId)
        {
            return await dbSet.Where(p => p.Name == name && p.UserId == userId && !p.IsDeleted).AnyAsync();
        }
    }
}
