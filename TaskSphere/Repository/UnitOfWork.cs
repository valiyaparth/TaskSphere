using Microsoft.AspNetCore.Identity;
using TaskSphere.Data;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        private UserManager<User> _userManager;

        public ITaskRepository Task { get; private set; }
        public IUserRepository User { get; private set; }
        public IProjectRepository Project { get; private set; }
        public ITeamRepository Team { get; private set; }

        public ITaskUserRepository TaskUser { get; private set; }
        public IProjectMemberRepository ProjectMember { get; private set; }
        public IProjectTeamRepository ProjectTeam { get; private set; }
        public ITeamMemberRepository TeamMember { get; private set; }

        public ITokenInfoRepository TokenInfo { get; private set; }

        public UnitOfWork(ApplicationDbContext db, UserManager<User> userManager) 
        {
            _db = db;
            _userManager = userManager;

            Task = new TaskRepository(_db);
            User = new UserRepository(_db, _userManager);
            Project = new ProjectRepository(_db);
            Team = new TeamRepository(_db);

            TaskUser = new TaskUserRepository(_db);
            TeamMember = new TeamMemberRepository(_db);
            ProjectMember = new ProjectMemberRepository(_db);
            ProjectTeam = new ProjectTeamRepository(_db);

            TokenInfo = new TokenInfoRepository(_db);

        }

        public async System.Threading.Tasks.Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
