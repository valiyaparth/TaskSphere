using TaskSphere.Data;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;  
        }

        public void Update(User user)
        {
            var oldUserObj = _db.Users.Where(u=>u.Id == user.Id).FirstOrDefault();
            if (oldUserObj != null) 
            {
                oldUserObj.Name = user.Name;
                oldUserObj.Email = user.Email;
                oldUserObj.Password = user.Password;
                if(oldUserObj.ImageUrl != null)
                {
                    oldUserObj.ImageUrl = user.ImageUrl;
                }
            }
        }
    }
}
