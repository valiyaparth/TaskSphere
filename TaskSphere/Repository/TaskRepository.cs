using Microsoft.EntityFrameworkCore;
using TaskSphere.Data;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class TaskRepository : Repository<Models.Task>, ITaskRepository
    {
        private ApplicationDbContext _db;
        public TaskRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Models.Task>> GetProjectTasksAsync(int projectId)
        {
            return await _db.Tasks.Where(u => u.ProjectId == projectId).ToListAsync();
        }

        public void Update(Models.Task obj)
        {
            var oldTaskObj = _db.Tasks.Where(u => u.Id == obj.Id).FirstOrDefault();
            if (oldTaskObj != null)
            {
                oldTaskObj.Title = obj.Title;
                oldTaskObj.Description = obj.Description;
                oldTaskObj.Status = obj.Status;
                oldTaskObj.DueDate = obj.DueDate;
                oldTaskObj.UpdatedAt = DateTime.Now;
                if (oldTaskObj.ImageUrl != null)
                {
                    oldTaskObj.ImageUrl = obj.ImageUrl;
                }

            }
        }
    }
}
