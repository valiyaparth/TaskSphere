using Microsoft.AspNetCore.Identity;
using TaskSphere.Models;

namespace TaskSphere.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        void Update(User user);
    }
}
