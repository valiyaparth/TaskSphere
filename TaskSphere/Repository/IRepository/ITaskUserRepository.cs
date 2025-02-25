using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface ITaskUserRepository
    {
        System.Threading.Tasks.Task AssignUserToTaskAsync(TaskUser taskUser);
        System.Threading.Tasks.Task RemoveUserFromTaskAsync(TaskUser taskUser);
    }
}
