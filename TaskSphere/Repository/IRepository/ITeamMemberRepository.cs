using System.Linq.Expressions;
using TaskSphere.Enums;
using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface ITeamMemberRepository
    {
        System.Threading.Tasks.Task AddUserToTeamAsync(TeamMember teamMember);
        Task<TeamMember?> GetAsync(Expression<Func<TeamMember, bool>> filter);
        Task<IEnumerable<TeamMember>> GetTeamMembersAsync(int id);
        System.Threading.Tasks.Task RemoveUserFromTeamAsync(TeamMember teamMember);

    }
}
