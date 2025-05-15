using Microsoft.AspNetCore.Identity;
using TaskSphere.Data;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;
        public UserRepository(ApplicationDbContext db, UserManager<User> userManager) : base(db)
        {
            _db = db;  
            _userManager = userManager;
        }


        public async Task<IdentityResult> CreateUserAsync(User user, string password)
        {
            if (_userManager == null)
            {
                throw new Exception("UserManager is null. Ensure dependency injection is set up properly.");
            }
            return await _userManager.CreateAsync(user, password);
        }

        public void Update(User user)
        {
            var oldUserObj = _db.Users.Where(u=>u.Id == user.Id).FirstOrDefault();
            if (oldUserObj != null) 
            {
                oldUserObj.Name = user.Name;
                oldUserObj.Email = user.Email;
                //oldUserObj.Password = user.Password;
                if(oldUserObj.ImageUrl != null)
                {
                    oldUserObj.ImageUrl = user.ImageUrl;
                }
            }
        }
    }
}
