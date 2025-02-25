using System.Linq.Expressions;
using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface IProjectTeamRepository
    {
        Task<ProjectTeam?> GetAsync(Expression<Func<ProjectTeam, bool>> filter);
        System.Threading.Tasks.Task AddTeamToProjectAsync(ProjectTeam projectTeam);
        System.Threading.Tasks.Task RemoveTeamFromProjectAsync(ProjectTeam projectTeam);
        Task<IEnumerable<ProjectTeam>> GetProjectTeamAsync(int id);
    }
}
