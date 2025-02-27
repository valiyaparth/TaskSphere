using System.Linq.Expressions;
using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface ITaskUserRepository
    {
        Task<TaskUser?> GetAsync(Expression<Func<TaskUser, bool>> filter);
        System.Threading.Tasks.Task AssignUserToTaskAsync(TaskUser taskUser);
        System.Threading.Tasks.Task RemoveUserFromTaskAsync(TaskUser taskUser);
    }
}
