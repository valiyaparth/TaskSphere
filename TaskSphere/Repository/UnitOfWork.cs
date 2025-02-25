using TaskSphere.Data;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public ITaskRepository Task { get; private set; }
        public IUserRepository User { get; private set; }
        public IProjectRepository Project { get; private set; }
        public ITeamRepository Team { get; private set; }

        public ITaskUserRepository TaskUser { get; private set; }
        public IProjectMemberRepository ProjectMember { get; private set; }
        public IProjectTeamRepository ProjectTeam { get; private set; }
        public ITeamMemberRepository TeamMember { get; private set; }

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Task = new TaskRepository(_db);
            User = new UserRepository(_db);
            Project = new ProjectRepository(_db);
            Team = new TeamRepository(_db);

            TaskUser = new TaskUserRepository(_db);
            TeamMember = new TeamMemberRepository(_db);
            ProjectMember = new ProjectMemberRepository(_db);
            ProjectTeam = new ProjectTeamRepository(_db);

        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
