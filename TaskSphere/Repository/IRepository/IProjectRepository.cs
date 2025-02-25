using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface IProjectRepository : IRepository<Project>
    {
        void Update(Project obj);
    }
}
