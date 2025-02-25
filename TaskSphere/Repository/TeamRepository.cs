using TaskSphere.Data;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        private ApplicationDbContext _db;

        public TeamRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Team team)
        {
            var oldTeamObj = _db.Teams.Where(u => u.Id == team.Id).FirstOrDefault();
            if (oldTeamObj != null)
            {
                oldTeamObj.Name = team.Name;
                oldTeamObj.Description = team.Description;
                oldTeamObj.UpdatedAt = DateTime.Now;
                if (oldTeamObj != null) 
                {
                    oldTeamObj.ImageUrl = team.ImageUrl;
                }
            }
        }

    }
}
