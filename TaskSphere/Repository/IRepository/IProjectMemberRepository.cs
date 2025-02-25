using System.Linq.Expressions;
using TaskSphere.Enums;
using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface IProjectMemberRepository
    {
        System.Threading.Tasks.Task AddUserToProjectAsync(ProjectMember projectMember);
        Task<ProjectMember?> GetAsync(Expression<Func<ProjectMember, bool>> filter);
        Task<IEnumerable<ProjectMember>> GetProjectMembersAsync(int id);
        System.Threading.Tasks.Task RemoveUserFromProjectAsync(ProjectMember projectMember);
    }
}
