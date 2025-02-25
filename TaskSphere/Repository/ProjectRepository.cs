using TaskSphere.Data;
using TaskSphere.Models;
using TaskSphere.Repository.IRepository;

namespace TaskSphere.Repository
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        private ApplicationDbContext _db;
        public ProjectRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Project project)
        {
            var oldProjectObj = _db.Projects.Where(u => u.Id == project.Id).FirstOrDefault();
            if (oldProjectObj != null)
            {
                oldProjectObj.Name = project.Name;
                oldProjectObj.Description = project.Description;
                oldProjectObj.UpdatedAt = DateTime.Now;
                if(oldProjectObj.ImageUrl != null)
                {
                    oldProjectObj.ImageUrl = project.ImageUrl;
                }
            }
        }
    }

}
