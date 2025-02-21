using TaskSphere.Enums;

namespace TaskSphere.Models
{
    public class ProjectMember
    {
        //composite key
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        //navigation properties
        public Project Project { get; set; }
        public User User { get; set; }

        //Role in the project
        public Roles Role { get; set; }
    }
}
