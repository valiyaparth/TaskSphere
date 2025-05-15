using TaskSphere.Data;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class TokenInfoRepository : Repository<TokenInfo>, ITokenInfoRepository
    {
        private readonly ApplicationDbContext _db;
        public TokenInfoRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
