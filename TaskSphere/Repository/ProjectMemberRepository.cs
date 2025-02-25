using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskSphere.Data;
using TaskSphere.Enums;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private ApplicationDbContext _db;

        public ProjectMemberRepository(ApplicationDbContext db) 
        { 
            _db = db; 
        }

        public async System.Threading.Tasks.Task AddUserToProjectAsync(ProjectMember projectMember)
        {
            await _db.ProjectMembers.AddAsync(projectMember);
        }

        public async Task<ProjectMember?> GetAsync(Expression<Func<ProjectMember, bool>> filter = null)
        {
            return await _db.ProjectMembers.FirstOrDefaultAsync(filter);
        }

        public async Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(int projectId)
        {
            return await _db.ProjectMembers.Where(u=>u.ProjectId == projectId).ToListAsync();
        }

        public async System.Threading.Tasks.Task RemoveUserFromProjectAsync(ProjectMember projectMember)
        {
            if (projectMember != null)
            {
                _db.ProjectMembers.Remove(projectMember);
            }
        }
    }
}
