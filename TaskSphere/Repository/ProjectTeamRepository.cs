using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskSphere.Data;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class ProjectTeamRepository : IProjectTeamRepository
    {
        private ApplicationDbContext _db;

        public ProjectTeamRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async System.Threading.Tasks.Task AddTeamToProjectAsync(ProjectTeam projectTeam)
        {
            await _db.ProjectTeams.AddAsync(projectTeam);
        }

        public async Task<ProjectTeam?> GetAsync(Expression<Func<ProjectTeam, bool>> filter)
        {
            return await _db.ProjectTeams.FirstOrDefaultAsync(filter);
        }

        public async Task<IEnumerable<ProjectTeam>> GetProjectTeamAsync(int id)
        {
            return await _db.ProjectTeams.Where(p => p.ProjectId == id).ToListAsync();
        }

        public async System.Threading.Tasks.Task RemoveTeamFromProjectAsync(ProjectTeam projectTeam)
        {
            if (projectTeam != null)
            {
                _db.ProjectTeams.Remove(projectTeam);
            }
        }
    }
}
