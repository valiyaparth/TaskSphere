using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskSphere.Data;
using TaskSphere.Enums;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class TeamMemberRepository : ITeamMemberRepository
    {
        ApplicationDbContext _db;

        public TeamMemberRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async System.Threading.Tasks.Task AddUserToTeamAsync(TeamMember teamMember)
        {
            await _db.TeamMembers.AddAsync(teamMember);
        }

        public async Task<TeamMember?> GetAsync(Expression<Func<TeamMember, bool>> filter)
        {
            return await _db.TeamMembers.FirstOrDefaultAsync(filter);
        }

        public async Task<IEnumerable<TeamMember>> GetTeamMembersAsync(int id)
        {
            return await _db.TeamMembers.Where(t => t.TeamId == id).ToListAsync();
        }

        public async System.Threading.Tasks.Task RemoveUserFromTeamAsync(TeamMember teamMember)
        {
            
            if (teamMember != null)
            {
                _db.TeamMembers.Remove(teamMember);
            }
        }
    }
}
