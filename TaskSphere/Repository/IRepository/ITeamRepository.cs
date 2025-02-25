using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface ITeamRepository : IRepository<Team>
    {
        void Update(Team team);
    }
}
