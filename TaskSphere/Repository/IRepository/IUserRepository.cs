using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        void Update(User user);
    }
}
