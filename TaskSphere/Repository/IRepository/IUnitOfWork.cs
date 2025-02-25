namespace TaskSphere.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ITaskRepository Task { get; }
        IUserRepository User { get; }
        IProjectRepository Project { get; }
        ITeamRepository Team { get; }

        ITaskUserRepository TaskUser { get; }
        ITeamMemberRepository TeamMember { get; }
        IProjectMemberRepository ProjectMember { get; }
        IProjectTeamRepository ProjectTeam { get; }

        Task SaveAsync();
    }
}
