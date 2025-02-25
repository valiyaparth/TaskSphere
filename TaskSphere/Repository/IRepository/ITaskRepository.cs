using TaskSphere.Models;
namespace TaskSphere.Repository.IRepository
{
    public interface ITaskRepository : IRepository<Models.Task>
    {
        void Update(Models.Task obj);
    }
}
