using TaskSphere.Models;
namespace TaskSphere.Repository.IRepository
{
    public interface ITaskRepository : IRepository<Models.Task>
    {
        Task<IEnumerable<Models.Task>> GetProjectTasksAsync(int projectId);
        void Update(Models.Task obj);
    }
}
